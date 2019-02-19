using System;
using System.Net;

namespace FastDFS.Client
{
    internal class QUERY_FILE_INFO : FDFSRequest
	{
		public static readonly QUERY_FILE_INFO Instance = new QUERY_FILE_INFO();

		private QUERY_FILE_INFO()
		{
		}

		public override FDFSRequest GetRequest(params object[] paramList)
		{
			if ((int)paramList.Length != 3)
			{
				throw new FDFSException("param count is wrong");
			}
			IPEndPoint pEndPoint = (IPEndPoint)paramList[0];
			string groupName = (string)paramList[1];
			string fileName = (string)paramList[2];
			QUERY_FILE_INFO queryFileInfo = new QUERY_FILE_INFO
            {
			    ConnectionType = FDFSConnectionType.StorageConnection,
			    EndPoint = pEndPoint
            };
            var groupNameByteCount = Util.StringByteCount(groupName);
            if (groupNameByteCount > 16)
            {
                throw new FDFSException("groupName is too long");
            }
            var fileNameByteCount = Util.StringByteCount(fileName);
            int length = 16 + fileNameByteCount;
            queryFileInfo.SetBodyBuffer(length);
            Util.StringToByte(groupName, queryFileInfo.BodyBuffer, 0, groupNameByteCount);
            Util.StringToByte(fileName, queryFileInfo.BodyBuffer, 16, fileNameByteCount);
            queryFileInfo.Header = new FDFSHeader(length, FDFSConstants.STORAGE_PROTO_CMD_QUERY_FILE_INFO, 0);
			return queryFileInfo;
		}
	}

    public class FDFSFileInfo : IFDFSResponse
    {
        public long FileSize { get; private set; }

        public DateTime CreateTime { get; private set; }

        public long Crc32 { get; private set; }

        public void ParseBuffer(byte[] responseBytes, int length)
        {
            this.FileSize = Util.BufferToLong(responseBytes, 0);
            DateTime dateTime = new DateTime(1970, 1, 1);
            this.CreateTime = dateTime.AddSeconds(Util.BufferToLong(responseBytes, 8)).ToLocalTime();
            this.Crc32 = Util.BufferToLong(responseBytes, 16);
        }
    }
}