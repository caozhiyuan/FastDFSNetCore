using System;
using System.Net;

namespace FastDFS.Client
{
    internal class QUERY_FILE_INFO : FDFSRequest
	{
		private static QUERY_FILE_INFO _instance;

		public static QUERY_FILE_INFO Instance
		{
			get
			{
				if (QUERY_FILE_INFO._instance == null)
				{
					QUERY_FILE_INFO._instance = new QUERY_FILE_INFO();
				}
				return QUERY_FILE_INFO._instance;
			}
		}

		static QUERY_FILE_INFO()
		{
			QUERY_FILE_INFO._instance = null;
		}

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
			string str = (string)paramList[1];
			string str1 = (string)paramList[2];
			QUERY_FILE_INFO queryFileInfo = new QUERY_FILE_INFO()
			{
			    ConnectionType = 1,
			    EndPoint = pEndPoint
            };
			if (str.Length > 16)
			{
				throw new FDFSException("groupName is too long");
			}
			long length = 16 + str1.Length;
            byte[] numArray = new byte[length];
			byte[] num = Util.StringToByte(str);
			byte[] num1 = Util.StringToByte(str1);
			Array.Copy(num, 0, numArray, 0, (int)num.Length);
			Array.Copy(num1, 0, numArray, 16, (int)num1.Length);
			queryFileInfo.Body = numArray;
			queryFileInfo.Header = new FDFSHeader(length, 22, 0);
			return queryFileInfo;
		}
	}
}