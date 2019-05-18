using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    internal class Connection
    {
        private const int DEFAULT_BUFFER_SIZE = 1024 * 64;

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

        private readonly EndPoint _endPoint;

        private Socket Socket { get; set; }

        public Connection(EndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        internal void ReUse()
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

        internal async Task SendExAsync(Stream stream)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(DEFAULT_BUFFER_SIZE);
            try
            {
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await Socket.SendExAsync(buffer, 0, read);
                }
            }
            catch (Exception)
            {
                await CloseSocketAsync();
                throw;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        internal async Task<int> ReceiveExAsync(byte[] buffer, int length)
        {
            var sent = await Socket.ReceiveExAsync(buffer, length);
            if (sent == 0)
            {
                await CloseSocketAsync();
            }

            return sent;
        }

        private async Task CloseSocketAsync()
        {
            var header = new FDFSHeader(0L, FDFSConstants.FDFS_PROTO_CMD_QUIT, 0);
            try
            {
                var headerArray = header.GetBuffer();
                await Socket.SendExAsync(new List<ArraySegment<byte>>
                {
                    headerArray
                });
                Socket.Close();
                Socket.Dispose();
            }
            catch
            {
                // ignored
            }
            finally
            {
                header.Dispose();
            }

            Socket = null;
        }
    }
}