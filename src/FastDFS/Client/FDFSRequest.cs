using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    internal class FDFSRequest
    {
        protected enum FDFSConnectionType
        {
            TrackerConnection = 0,
            StorageConnection = 1
        }

        protected FDFSConnectionType ConnectionType { get; set; }

        private Connection _connection;

        protected IPEndPoint EndPoint { get; set; }

        protected FDFSHeader Header { get; set; }

        protected byte[] BodyBuffer { get; set; }

        protected FDFSRequest()
        {
            ConnectionType = FDFSConnectionType.TrackerConnection;
        }

        protected void SetBodyBuffer(int length)
        {
            BodyBuffer = ArrayPool<byte>.Shared.Rent(length);
        }

        public virtual FDFSRequest GetRequest(params object[] paramList)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetResponseAsync<T>() where T: IFDFSResponse, new ()
        {
            try
            {
                if (ConnectionType == FDFSConnectionType.TrackerConnection)
                {
                    _connection = await ConnectionManager.GetTrackerConnectionAsync();
                }
                else
                {
                    _connection = await ConnectionManager.GetStorageConnectionAsync(EndPoint);
                }
                await _connection.OpenAsync();

                var headerBuffer = Header.GetBuffer();
                var buffers = new List<ArraySegment<byte>>(2)
                {
                    headerBuffer,
                    new ArraySegment<byte>(BodyBuffer, 0, (int) Header.Length)
                };
                await _connection.SendExAsync(buffers);
                
                var responseHeader = await GetResponseHeaderInfo<T>(headerBuffer);

                return await GetResponseInfo<T>(responseHeader);
            }
            finally
            {
                if (BodyBuffer != null)
                {
                    ArrayPool<byte>.Shared.Return(BodyBuffer);
                }

                Header?.Dispose();

                _connection?.ReUse();
            }
        }

        private async Task<FDFSHeader> GetResponseHeaderInfo<T>(ArraySegment<byte> arraySegment) where T : IFDFSResponse, new()
        {
            var headerArray = arraySegment.Array;
            if (headerArray == null)
            {
                throw new ArgumentNullException(nameof(arraySegment.Array));
            }

            if (await _connection.ReceiveExAsync(headerArray, arraySegment.Count) == 0)
            {
                throw new FDFSException("Init Header Exception : Can't Read Stream");
            }

            var length = Util.BufferToLong(headerArray, 0);
            var command = headerArray[8];
            var status = headerArray[9];
            return new FDFSHeader(length, command, status);
        }

        private async Task<T> GetResponseInfo<T>(FDFSHeader responseHeader) where T : IFDFSResponse, new()
        {
            if (responseHeader.Status != 0)
            {
                if (Header.Command == FDFSConstants.STORAGE_PROTO_CMD_QUERY_FILE_INFO)
                {
                    return default(T);
                }
                throw new FDFSStatusException(responseHeader.Status, $"Get Response Error, Error Code:{(FDFSConstants.ErrorCode)responseHeader.Status}");
            }

            if (responseHeader.Length <= 0)
            {
                return default(T);
            }

            int resLen = (int) responseHeader.Length;
            var resBuffer = ArrayPool<byte>.Shared.Rent(resLen);
            try
            {
                await _connection.ReceiveExAsync(resBuffer, resLen);
                var response = new T();
                response.ParseBuffer(resBuffer, resLen);
                return response;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(resBuffer);
                responseHeader.Dispose();
            }
        }
    }
}