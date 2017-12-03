using System;
using System.Text;

namespace FastDFS.Client
{
    internal class QUERY_STORE_WITH_GROUP_ONE : FDFSRequest
	{
		private static QUERY_STORE_WITH_GROUP_ONE _instance;

		public static QUERY_STORE_WITH_GROUP_ONE Instance
		{
			get
			{
				if (QUERY_STORE_WITH_GROUP_ONE._instance == null)
				{
					QUERY_STORE_WITH_GROUP_ONE._instance = new QUERY_STORE_WITH_GROUP_ONE();
				}
				return QUERY_STORE_WITH_GROUP_ONE._instance;
			}
		}

		static QUERY_STORE_WITH_GROUP_ONE()
		{
			QUERY_STORE_WITH_GROUP_ONE._instance = null;
		}

		private QUERY_STORE_WITH_GROUP_ONE()
		{
		}

		public override FDFSRequest GetRequest(params object[] paramList)
		{
			if ((int)paramList.Length == 0)
			{
				throw new FDFSException("GroupName is null");
			}
			QUERY_STORE_WITH_GROUP_ONE queryStoreWithGroupOne = new QUERY_STORE_WITH_GROUP_ONE();
			byte[] num = Util.StringToByte((string)paramList[0]);
			if ((int)num.Length > 16)
			{
				throw new FDFSException("GroupName is too long");
			}
			byte[] numArray = new byte[16];
			Array.Copy(num, 0, numArray, 0, (int)num.Length);
			queryStoreWithGroupOne.Body = numArray;
			queryStoreWithGroupOne.Header = new FDFSHeader((long)16, 104, 0);
			return queryStoreWithGroupOne;
		}

		public class Response
		{
			public string GroupName;

			public string IPStr;

			public int Port;

			public byte StorePathIndex;

			public Response(byte[] responseByte)
			{
				byte[] numArray = new byte[16];
				Array.Copy(responseByte, numArray, 16);
				this.GroupName = Util.ByteToString(numArray).TrimEnd(new char[1]);
				byte[] numArray1 = new byte[15];
				Array.Copy(responseByte, 16, numArray1, 0, 15);
				this.IPStr = (new string(FDFSConfig.Charset.GetChars(numArray1))).TrimEnd(new char[1]);
				byte[] numArray2 = new byte[8];
				Array.Copy(responseByte, 31, numArray2, 0, 8);
				this.Port = (int)Util.BufferToLong(numArray2, 0);
				this.StorePathIndex = responseByte[(int)responseByte.Length - 1];
			}
		}
	}
}