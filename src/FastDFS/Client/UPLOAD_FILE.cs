using System;
using System.IO;
using System.Net;

namespace FastDFS.Client
{
    internal class UPLOAD_FILE : FDFSRequest
    {
        public static readonly UPLOAD_FILE Instance = new UPLOAD_FILE();

        private UPLOAD_FILE()
        {
        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
            {
                throw new FDFSException("param count is wrong");
            }

            StorageNode storageNode = (StorageNode) paramList[0];
            string fileExt = (string) paramList[1] ?? string.Empty;
            if (fileExt.Length > 0 && fileExt[0] == '.')
            {
                fileExt = fileExt.Substring(1);
            }
            var contentStream = (Stream) paramList[2];

            var contentByteLength = contentStream.Length;

            UPLOAD_FILE uploadFile = new UPLOAD_FILE
            {
                ConnectionType = FDFSConnectionType.StorageConnection,
                EndPoint = storageNode.EndPoint
            };
            if (fileExt.Length > 6)
            {
                throw new FDFSException("file ext is too long");
            }

            const int bodyBufferLen = 15;
            uploadFile.SetBodyBuffer(bodyBufferLen);

            int offset = 0;
            uploadFile.BodyBuffer[offset++] = storageNode.StorePathIndex;

            Util.LongToBuffer(contentByteLength, uploadFile.BodyBuffer, offset);
            offset += 8;

            var fileExtByteCount = Util.StringByteCount(fileExt);
            Util.StringToByte(fileExt, uploadFile.BodyBuffer, offset, fileExtByteCount);
            if (fileExtByteCount < 6)
            {
                for (var i = offset + fileExtByteCount; i < offset + 6; i++)
                {
                    uploadFile.BodyBuffer[i] = 0;
                }
            }

            uploadFile.BodyStream = contentStream;

            long length = bodyBufferLen + contentStream.Length;
            uploadFile.Header = new FDFSHeader(length, FDFSConstants.STORAGE_PROTO_CMD_UPLOAD_FILE, 0);
            return uploadFile;
        }
        
        public class Response:IFDFSResponse
        {
            public string FileName { get; private set; }

            public string GroupName { get; private set; }

            public void ParseBuffer(byte[] responseByte, int length)
            {
                this.GroupName = Util.ByteToString(responseByte, 0, 16);
                this.FileName = Util.ByteToString(responseByte, 16, length - 16);
            }
        }
    }
}
