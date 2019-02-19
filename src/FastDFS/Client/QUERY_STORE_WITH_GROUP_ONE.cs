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
            var groupName = (string) paramList[0];
            var groupNameByteCount = Util.StringByteCount(groupName);
			if (groupNameByteCount > 16)
			{
				throw new FDFSException("GroupName is too long");
			}

            const int length = 16;
            queryStoreWithGroupOne.SetBodyBuffer(length);
            Util.StringToByte(groupName, queryStoreWithGroupOne.BodyBuffer, 0, groupNameByteCount);
            queryStoreWithGroupOne.Header = new FDFSHeader(length, FDFSConstants.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE, 0);
			return queryStoreWithGroupOne;
		}

		public class Response: IFDFSResponse
		{
			public string GroupName { get; private set; }

            public string IPStr { get; private set; }

            public int Port { get; private set; }

            public byte StorePathIndex { get; private set; }

            public void ParseBuffer(byte[] responseByte, int length)
            {
                this.GroupName = Util.ByteToString(responseByte, 0, 16);
                this.IPStr = Util.ByteToString(responseByte, 16, 15);
                this.Port = (int)Util.BufferToLong(responseByte, 31);
                this.StorePathIndex = responseByte[length - 1];
            }
        }
	}
}