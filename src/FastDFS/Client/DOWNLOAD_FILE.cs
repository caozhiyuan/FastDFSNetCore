using System;

namespace FastDFS.Client
{
    internal class DOWNLOAD_FILE : FDFSRequest
    {
        public static readonly DOWNLOAD_FILE Instance = new DOWNLOAD_FILE();

        private DOWNLOAD_FILE()
        {
        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
                throw new FDFSException("param count is wrong");

            var storageNode = (StorageNode)paramList[0];
            var fileName = (string)paramList[1];
            var offsetInfo = (Tuple<long, long>)paramList[2];

            var downloadFile = new DOWNLOAD_FILE
            {
                ConnectionType = FDFSConnectionType.StorageConnection,
                EndPoint = storageNode.EndPoint
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

        public class Response : IFDFSResponse
        {
            public byte[] Content { get; set; }

            public void ParseBuffer(byte[] responseByte, int length)
            {
                Content = new byte[length];
                Array.Copy(responseByte, 0, Content, 0, length);
            }
        }
    }
}
