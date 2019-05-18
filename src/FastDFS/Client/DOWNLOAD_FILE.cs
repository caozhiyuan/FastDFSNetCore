using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    internal class DOWNLOAD_FILE : FDFSRequest
    {
        private const int DEFAULT_BUFFER_SIZE = 1024 * 256;

        public IDownloadCallback Callback { get; set; }

        public static readonly DOWNLOAD_FILE Instance = new DOWNLOAD_FILE();

        private DOWNLOAD_FILE()
        {

        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 4)
                throw new FDFSException("param count is wrong");

            var storageNode = (StorageNode)paramList[0];
            var fileName = (string)paramList[1];
            var offsetInfo = (Tuple<long, long>)paramList[2];

            var downloadFile = new DOWNLOAD_FILE
            {
                ConnectionType = FDFSConnectionType.StorageConnection,
                EndPoint = storageNode.EndPoint,
                Callback = (IDownloadCallback) paramList[3]
            };

            var groupNameByteCount = Util.StringByteCount(storageNode.GroupName);
            if (groupNameByteCount > 16)
            {
                throw new FDFSException("groupName is too long");
            }

            var fileNameByteCount = Util.StringByteCount(fileName);
            int length = 16 + 16 + fileNameByteCount;

            downloadFile.SetBodyBuffer(length);

            int offset = 0;

            Util.LongToBuffer(offsetInfo.Item1, downloadFile.BodyBuffer, offset);
            offset += 8;

            Util.LongToBuffer(offsetInfo.Item2, downloadFile.BodyBuffer, offset);
            offset += 8;

            Util.StringToByte(storageNode.GroupName, downloadFile.BodyBuffer, offset, groupNameByteCount);
            if (groupNameByteCount < 16)
            {
                for (var i = offset + groupNameByteCount; i < offset + 16; i++)
                {
                    downloadFile.BodyBuffer[i] = 0;
                }
            }
            offset += 16;

            Util.StringToByte(fileName, downloadFile.BodyBuffer, offset, fileNameByteCount);
      
            downloadFile.Header = new FDFSHeader(length, FDFSConstants.STORAGE_PROTO_CMD_DOWNLOAD_FILE, 0);
            return downloadFile;
        }

        protected override async Task<T> ParseResponseInfo<T>(FDFSHeader responseHeader)
        {
            var buff = ArrayPool<byte>.Shared.Rent(DEFAULT_BUFFER_SIZE);
            try
            {
                var remainBytes = responseHeader.Length;
                while (remainBytes > 0)
                {
                    var bytes = await _connection.ReceiveExAsync(buff, remainBytes > buff.Length ? buff.Length : (int)remainBytes);
                    int result;
                    if ((result = await Callback.ReceiveAsync(responseHeader.Length, buff, bytes)) != 0)
                    {
                        throw new FDFSStatusException(responseHeader.Status, $"Callback Receive Error:{result}");
                    }
                    remainBytes -= bytes;
                }             
                return new T();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buff);
                responseHeader.Dispose();
            }
        }

        public class Response : IFDFSResponse
        {
            public void ParseBuffer(byte[] responseByte, int length)
            {
            }
        }
    }

    public interface IDownloadCallback
    {
        Task<int> ReceiveAsync(long fileSize, byte[] data, int bytes);
    }

    public class StreamDownloadCallback : IDownloadCallback
    {
        private readonly Stream _stream;

        public StreamDownloadCallback(Stream stream)
        {
            _stream = stream;
        }

        public async Task<int> ReceiveAsync(long fileSize, byte[] data, int bytes)
        {
            await _stream.WriteAsync(data, 0, bytes);
            return 0;
        }
    }
}
