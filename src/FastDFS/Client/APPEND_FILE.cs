using System;
using System.Net;

namespace FastDFS.Client
{
    internal class APPEND_FILE : FDFSRequest
    {
        public static readonly APPEND_FILE Instance = new APPEND_FILE();

        private APPEND_FILE()
        {
        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
                throw new FDFSException("param count is wrong");
            var endPoint = (IPEndPoint) paramList[0];

            var fileName = (string) paramList[1];
            byte[] contentByte = (byte[]) paramList[2];

            var appendFile = new APPEND_FILE
            {
                ConnectionType = FDFSConnectionType.StorageConnection,
                EndPoint = endPoint
            };

            var fileNameByteCount = Util.StringByteCount(fileName);
            int length = 16 + fileNameByteCount + contentByte.Length;
            appendFile.SetBodyBuffer(length);

            int offset = 0;

            Util.LongToBuffer(fileNameByteCount, appendFile.BodyBuffer, offset);
            offset += 8;

            Util.LongToBuffer(contentByte.Length, appendFile.BodyBuffer, offset);
            offset += 8;

            Util.StringToByte(fileName, appendFile.BodyBuffer, offset, fileNameByteCount);
            offset += fileNameByteCount;

            Array.Copy(contentByte, 0, appendFile.BodyBuffer, offset, contentByte.Length);

            appendFile.Header = new FDFSHeader(length, FDFSConstants.STORAGE_PROTO_CMD_APPEND_FILE, 0);
            return appendFile;
        }

        public class Response : IFDFSResponse
        {
            public void ParseBuffer(byte[] responseByte, int length)
            {
            }
        }
    }
}
