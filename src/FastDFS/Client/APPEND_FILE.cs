using System;
using System.IO;
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
            var contentStream = (Stream) paramList[2];

            var appendFile = new APPEND_FILE
            {
                ConnectionType = FDFSConnectionType.StorageConnection,
                EndPoint = endPoint
            };

            var fileNameByteCount = Util.StringByteCount(fileName);
            int bodyBufferLen = 16 + fileNameByteCount;
            appendFile.SetBodyBuffer(bodyBufferLen);

            int offset = 0;

            Util.LongToBuffer(fileNameByteCount, appendFile.BodyBuffer, offset);
            offset += 8;

            Util.LongToBuffer(contentStream.Length, appendFile.BodyBuffer, offset);
            offset += 8;

            Util.StringToByte(fileName, appendFile.BodyBuffer, offset, fileNameByteCount);

            appendFile.BodyStream = contentStream;

            long length = bodyBufferLen + contentStream.Length;
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
