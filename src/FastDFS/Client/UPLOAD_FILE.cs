using System;
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
            string fileExt = (string) paramList[1];
            byte[] contentByte = (byte[]) paramList[2];

            int contentByteLength = contentByte.Length;

            UPLOAD_FILE uploadFile = new UPLOAD_FILE
            {
                ConnectionType = FDFSConnectionType.StorageConnection,
                EndPoint = storageNode.EndPoint
            };
            if (fileExt.Length > 6)
            {
                throw new FDFSException("file ext is too long");
            }

            int length = 15 + contentByteLength;
            uploadFile.SetBodyBuffer(length);

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

            offset += 6;
            Array.Copy(contentByte, 0, uploadFile.BodyBuffer, offset, contentByte.Length);

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