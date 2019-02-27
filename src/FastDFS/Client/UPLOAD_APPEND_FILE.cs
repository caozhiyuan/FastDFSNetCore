using System;
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
            string fileExt = (string)paramList[1];
            byte[] contentByte = (byte[])paramList[2];

            int contentByteLength = contentByte.Length;

            UPLOAD_APPEND_FILE uploadAppendFile = new UPLOAD_APPEND_FILE
			{
			    ConnectionType = FDFSConnectionType.StorageConnection,
			    EndPoint = storageNode.EndPoint
            };

            if (fileExt.Length > 6)
            {
                throw new FDFSException("file ext is too long");
            }
            int length = 15 + contentByteLength;
            uploadAppendFile.SetBodyBuffer(length);

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

            offset += 6;
            Array.Copy(contentByte, 0, uploadAppendFile.BodyBuffer, offset, contentByte.Length);

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