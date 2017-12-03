using System;
using System.Net;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    internal class FDFSRequest
    {
        private FDFSHeader _header;

        private byte[] _body;

        private Connection _connection;

        protected byte[] Body
        {
            get => this._body;
            set => this._body = value;
        }

        protected Connection Connection
        {
            get => this._connection;
            set => this._connection = value;
        }

        protected FDFSHeader Header
        {
            get => this._header;
            set => this._header = value;
        }

        protected int ConnectionType { get; set; }
        protected IPEndPoint EndPoint { get; set; }

        protected FDFSRequest()
        {
        }

        public virtual FDFSRequest GetRequest(params object[] paramList)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] GetResponse()
        {
            if (this.ConnectionType == 0)
            {
                this._connection = ConnectionManager.GetTrackerConnection();
            }
            else
            {
                this._connection = ConnectionManager.GetStorageConnection(EndPoint);
            }
            try
            {
                this._connection.Open();

                byte[] num = this._header.ToByte();
                _connection.SendEx(num);
                _connection.SendEx(this._body);

                byte[] numArray0 = new byte[10];
                if (_connection.ReceiveEx(numArray0) == 0)
                {
                    throw new FDFSException("Init Header Exeption : Cann't Read Stream");
                }
                var length = Util.BufferToLong(numArray0, 0);
                var command = numArray0[8];
                var status = numArray0[9];

                var fDFSHeader = new FDFSHeader(length, command, status);
                if (fDFSHeader.Status != 0)
                {
                    throw new FDFSException($"Get Response Error,Error Code:{fDFSHeader.Status}");
                }
                byte[] numArray = new byte[fDFSHeader.Length];
                if (fDFSHeader.Length != (long) 0)
                {
                    _connection.ReceiveEx(numArray);
                }
                return numArray;
            }
            finally
            {
                this._connection.Close();
            }
        }

        public virtual async Task<byte[]> GetResponseAsync()
        {
            if (this.ConnectionType == 0)
            {
                this._connection = await ConnectionManager.GetTrackerConnectionAsync();
            }
            else
            {
                this._connection = await ConnectionManager.GetStorageConnectionAsync(EndPoint);
            }
            try
            {
                await this._connection.OpenAsync();

                byte[] num = this._header.ToByte();
                await _connection.SendExAsync(num);
                await _connection.SendExAsync(this._body);

                byte[] numArray0 = new byte[10];
                if (await _connection.ReceiveExAsync(numArray0) == 0)
                {
                    throw new FDFSException("Init Header Exeption : Cann't Read Stream");
                }
                var length = Util.BufferToLong(numArray0, 0);
                var command = numArray0[8];
                var status = numArray0[9];

                var fDFSHeader = new FDFSHeader(length, command, status);
                if (fDFSHeader.Status != 0)
                {
                    throw new FDFSException($"Get Response Error,Error Code:{fDFSHeader.Status}");
                }
                byte[] numArray = new byte[fDFSHeader.Length];
                if (fDFSHeader.Length != (long) 0)
                {
                    await _connection.ReceiveExAsync(numArray);
                }
                return numArray;
            }
            finally
            {
                this._connection.Close();
            }
        }

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }
    }
}