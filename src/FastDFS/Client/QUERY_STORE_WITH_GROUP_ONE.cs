using System;

namespace FastDFS.Client
{
    internal class QUERY_STORE_WITH_GROUP_ONE : FDFSRequest
	{
		public static readonly QUERY_STORE_WITH_GROUP_ONE Instance = new QUERY_STORE_WITH_GROUP_ONE();

		private QUERY_STORE_WITH_GROUP_ONE()
		{
		}

		public override FDFSRequest GetRequest(params object[] paramList)
		{
			if (paramList.Length == 0)
			{
				throw new FDFSException("GroupName is null");
			}
			QUERY_STORE_WITH_GROUP_ONE queryStoreWithGroupOne = new QUERY_STORE_WITH_GROUP_ONE();
			var groupNameArray = Util.StringToByte((string)paramList[0]);
			if (groupNameArray.Length > 16)
			{
				throw new FDFSException("GroupName is too long");
			}

            const int length = 16;
            queryStoreWithGroupOne.SetBodyBuffer(length);
            groupNameArray.CopyTo(new Span<byte>(queryStoreWithGroupOne.BodyBuffer, 0, groupNameArray.Length));
			queryStoreWithGroupOne.Header = new FDFSHeader(length, 104, 0);
			return queryStoreWithGroupOne;
		}

		public class Response: IFDFSResponse
		{
			public string GroupName { get; private set; }

            public string IPStr { get; private set; }

            public int Port { get; private set; }

            public byte StorePathIndex { get; private set; }

            public void ParseBuffer(byte[] responseByte)
            {
                Span<byte> span = new Span<byte>(responseByte);
                this.GroupName = Util.ByteToString(span.Slice(0, 16).ToArray());
                this.IPStr = Util.ByteToString(span.Slice(16, 15).ToArray());
                this.Port = (int)Util.BufferToLong(responseByte, 31);
                this.StorePathIndex = responseByte[responseByte.Length - 1];
            }
        }
	}
}