using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    internal class Connection
	{
		private Pool _pool;

	    private DateTime _lastUseTime;

        private bool _inUse = false;

	    public bool InUse
		{
			get => this._inUse;
	        set => this._inUse = value;
	    }

		public Pool Pool
		{
			get => this._pool;
		    set => this._pool = value;
		}

	    private readonly IPEndPoint _endPoint;

        private Socket Socket { get; set; }

	    public Connection(IPEndPoint endPoint)
	    {
	        _endPoint = endPoint;
        }

	    internal void Close()
		{
			this._pool.CloseConnection(this);
		}

	    internal void Open()
		{
			if (this._inUse)
			{
				throw new FDFSException("the connection is already in user");
			}

		    if (_lastUseTime != default(DateTime) && IdleTotalSeconds > FDFSConfig.ConnectionLifeTime)
		    {
		        CloseSocket();
            }

		    this._inUse = true;
		    this._lastUseTime = DateTime.Now;

		    if (Socket == null || !Socket.Connected)
		    {
		        Socket = NewSocket();
                Socket.Connect(_endPoint);
		    }
		}

	    internal async Task OpenAsync()
	    {
	        if (this._inUse)
	        {
	            throw new FDFSException("the connection is already in user");
	        }

	        if (_lastUseTime != default(DateTime) && IdleTotalSeconds > FDFSConfig.ConnectionLifeTime)
	        {
	            await CloseSocketAsync();
	        }

            this._inUse = true;
	        this._lastUseTime = DateTime.Now;

            if (Socket == null || !Socket.Connected)
            {
                Socket = NewSocket();
                await Socket.ConnectExAsync(_endPoint);
	        }
	    }

	    private double IdleTotalSeconds
	    {
	        get { return (DateTime.Now - _lastUseTime).TotalSeconds; }
	    }

	    private Socket NewSocket()
	    {
	        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	        socket.LingerState = new LingerOption(false, 0);
	        socket.NoDelay = true;
	        return socket;
	    }


	    public int SendEx(byte[] buffer)
	    {
	        try
	        {
	            return Socket.Send(buffer, 0, (int)buffer.Length, SocketFlags.None);
            }
	        catch (SocketException)
	        {
	            CloseSocket();
                throw;
	        }
	    }

	    internal int ReceiveEx(byte[] buffer)
	    {
	        var sent = Socket.Receive(buffer, 0, (int)buffer.Length, SocketFlags.None);
	        if (sent == 0)
	        {
	            CloseSocket();
	        }
            return sent;
	    }

	    private void CloseSocket()
	    {
	        try
	        {
	            byte[] buffer0 = new FDFSHeader(0L, 0x52, 0).ToByte();
	            Socket.Send(buffer0, 0, (int) buffer0.Length, SocketFlags.None);
	            Socket.Close();
	            Socket.Dispose();
	        }
	        catch
	        {
	            // ignored
	        }
	        Socket = null;
	    }

	    internal async Task<int> SendExAsync(byte[] buffer)
	    {
	        try
	        {
	            return await Socket.SendExAsync(buffer);
            }
	        catch (Exception)
	        {
	            await CloseSocketAsync();
                throw;
	        }
	    }

	    internal async Task<int> ReceiveExAsync(byte[] buffer)
	    {
	        var sent = await Socket.ReceiveExAsync(buffer);
	        if (sent == 0)
	        {
	            await CloseSocketAsync();
	        }
	        return sent;
	    }

	    private async Task CloseSocketAsync()
	    {
	        try
	        {
	            byte[] buffer0 = new FDFSHeader(0L, 0x52, 0).ToByte();
	            await Socket.SendExAsync(buffer0);
	            Socket.Close();
	            Socket.Dispose();
	        }
	        catch
	        {
	            // ignored
	        }
	        Socket = null;
	    }
	}
}