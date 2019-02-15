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

            int length = 15 + contentByte.Length;
            uploadFile.SetBodyBuffer(length);

            int offset = 0;
            uploadFile.BodyBuffer[offset++] = storageNode.StorePathIndex;

            Util.LongToBuffer(contentByteLength, uploadFile.BodyBuffer, offset);
            offset += 8;

            var fileExtBuffer = Util.StringToByte(fileExt);
            var tempLen = Math.Min(fileExtBuffer.Length, 6);
            fileExtBuffer.Slice(0, tempLen).CopyTo(new Span<byte>(uploadFile.BodyBuffer, offset, tempLen));
            if (tempLen < 6)
            {
                for (var i = offset + tempLen; i < offset + 6; i++)
                {
                    uploadFile.BodyBuffer[i] = 0;
                }
            }

            offset += 6;
            Array.Copy(contentByte, 0, uploadFile.BodyBuffer, offset, contentByte.Length);

            uploadFile.Header = new FDFSHeader(length, 11, 0);
            return uploadFile;
        }
        
        public class Response:IFDFSResponse
        {
            public string FileName { get; private set; }

            public string GroupName { get; private set; }

            public void ParseBuffer(byte[] responseByte)
            {
                Span<byte> span = new Span<byte>(responseByte);
                this.GroupName = Util.ByteToString(span.Slice(0, 16).ToArray());
                this.FileName = Util.ByteToString(span.Slice(16, responseByte.Length - 16).ToArray());
            }
        }
    }
}