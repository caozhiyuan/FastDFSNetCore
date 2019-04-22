using System;

namespace FastDFS.Client
{
    internal class QUERY_STORE_WITHOUT_GROUP_ONE : FDFSRequest
	{
		public static readonly QUERY_STORE_WITHOUT_GROUP_ONE Instance = new QUERY_STORE_WITHOUT_GROUP_ONE();

		private QUERY_STORE_WITHOUT_GROUP_ONE()
		{
		}

		public override FDFSRequest GetRequest(params object[] paramList)
		{
            QUERY_STORE_WITHOUT_GROUP_ONE queryStoreWithGroupOne = new QUERY_STORE_WITHOUT_GROUP_ONE();
            queryStoreWithGroupOne.BodyBuffer = new byte[0];
            queryStoreWithGroupOne.Header = new FDFSHeader(0, FDFSConstants.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE, 0);
			return queryStoreWithGroupOne;
		}
	}
}