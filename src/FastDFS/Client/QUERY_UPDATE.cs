using System;

namespace FastDFS.Client
{
    internal class QUERY_UPDATE : FDFSRequest
	{
		public static readonly QUERY_UPDATE Instance = new QUERY_UPDATE();

		private QUERY_UPDATE()
		{
		}

		public override FDFSRequest GetRequest(params object[] paramList)
		{
			if (paramList.Length != 2)
			{
				throw new FDFSException("param count is wrong");
			}
			QUERY_UPDATE queryUpDate = new QUERY_UPDATE();
			string groupName = (string)paramList[0];
            string fileName = (string)paramList[1];

            var groupNameByteCount = Util.StringByteCount(groupName);
            if (groupNameByteCount > 16)
            {
                throw new FDFSException("groupName is too long");
            }
            var fileNameByteCount = Util.StringByteCount(fileName);
            int length = 16 + fileNameByteCount;
            queryUpDate.SetBodyBuffer(length);
            Util.StringToByte(groupName, queryUpDate.BodyBuffer, 0, groupNameByteCount);
            Util.StringToByte(fileName, queryUpDate.BodyBuffer, 16, fileNameByteCount);
            queryUpDate.Header = new FDFSHeader(length, FDFSConstants.TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE, 0);
			return queryUpDate;
		}

		public class Response: IFDFSResponse
		{
			public string GroupName { get; set; }

            public string IPStr { get; set; }

			public int Port { get; set; }

            public void ParseBuffer(byte[] responseByte, int length)
            {
                this.GroupName = Util.ByteToString(responseByte, 0, 16);
                this.IPStr = Util.ByteToString(responseByte, 16, 15);
                this.Port = (int)Util.BufferToLong(responseByte, 31);
            }
        }
	}
}