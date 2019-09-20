using System;
using System.IO;
using System.Net;

namespace FastDFS.Client
{
    internal class UPLOAD_APPEND_FILE : FDFSRequest
	{
        public static readonly UPLOAD_APPEND_FILE Instance = new UPLOAD_APPEND_FILE();

        private UPLOAD_APPEND_FILE()
        {
        }

		public override FDFSRequest GetRequest(params object[] paramList)
		{
			if (paramList.Length != 3)
			{
				throw new FDFSException("param count is wrong");
			}

            StorageNode storageNode = (StorageNode)paramList[0];
            string fileExt = (string) paramList[1] ?? string.Empty;
            if (fileExt.Length > 0 && fileExt[0] == '.')
            {
                fileExt = fileExt.Substring(1);
            }

            var contentStream = (Stream) paramList[2];

            var contentByteLength = contentStream.Length;

            UPLOAD_APPEND_FILE uploadAppendFile = new UPLOAD_APPEND_FILE
			{
			    ConnectionType = FDFSConnectionType.StorageConnection,
			    EndPoint = storageNode.EndPoint
            };

            if (fileExt.Length > 6)
            {
                throw new FDFSException("file ext is too long");
            }

            const int bodyBufferLen = 15;
            uploadAppendFile.SetBodyBuffer(bodyBufferLen);

            int offset = 0;
            uploadAppendFile.BodyBuffer[offset++] = storageNode.StorePathIndex;

            Util.LongToBuffer(contentByteLength, uploadAppendFile.BodyBuffer, offset);
            offset += 8;

            var fileExtByteCount = Util.StringByteCount(fileExt);
            Util.StringToByte(fileExt, uploadAppendFile.BodyBuffer, offset, fileExtByteCount);
            if (fileExtByteCount < 6)
            {
                for (var i = offset + fileExtByteCount; i < offset + 6; i++)
                {
                    uploadAppendFile.BodyBuffer[i] = 0;
                }
            }

            uploadAppendFile.BodyStream = contentStream;

            var length = bodyBufferLen + contentByteLength;
            uploadAppendFile.Header = new FDFSHeader(length, FDFSConstants.STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE, 0);
			return uploadAppendFile;
		}

		public class Response : IFDFSResponse
        {
			public string GroupName;

			public string FileName;

            public void ParseBuffer(byte[] responseByte, int length)
            {
                this.GroupName = Util.ByteToString(responseByte, 0, 16);
                this.FileName = Util.ByteToString(responseByte, 16, length - 16);
            }
        }
	}
}