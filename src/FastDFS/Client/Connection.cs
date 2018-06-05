using System;
using System.Collections.Generic;
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

        internal async Task<int> SendExAsync(IList<ArraySegment<byte>> buffers)
        {
            try
            {
                return await Socket.SendExAsync(buffers);
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