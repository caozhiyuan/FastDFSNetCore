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
			if (groupName.Length > 16)
			{
				throw new FDFSException("GroupName is too long");
			}
            var fileNameArray = Util.StringToByte(fileName);
            int length = 16 + fileNameArray.Length;
            queryUpDate.SetBodyBuffer(length);
            var groupNameArray = Util.StringToByte(groupName);
            groupNameArray.CopyTo(new Span<byte>(queryUpDate.BodyBuffer, 0, groupNameArray.Length));
            fileNameArray.CopyTo(new Span<byte>(queryUpDate.BodyBuffer, 16, fileNameArray.Length));
            queryUpDate.Header = new FDFSHeader(length, 103, 0);
			return queryUpDate;
		}

		public class Response: IFDFSResponse
		{
			public string GroupName { get; set; }

            public string IPStr { get; set; }

			public int Port { get; set; }

            public void ParseBuffer(byte[] responseByte, int length)
            {
                Span<byte> span = new Span<byte>(responseByte);
                this.GroupName = Util.ByteToString(span.Slice(0, 16).ToArray());
                this.IPStr = Util.ByteToString(span.Slice(16, 15).ToArray());
                this.Port = (int)Util.BufferToLong(responseByte, 31);
            }
        }
	}
}