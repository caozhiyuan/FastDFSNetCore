using System;
using System.Text;

namespace FastDFS.Client
{
    internal class QUERY_UPDATE : FDFSRequest
	{
		private static QUERY_UPDATE _instance;

		public static QUERY_UPDATE Instance
		{
			get
			{
				if (QUERY_UPDATE._instance == null)
				{
					QUERY_UPDATE._instance = new QUERY_UPDATE();
				}
				return QUERY_UPDATE._instance;
			}
		}

		static QUERY_UPDATE()
		{
			QUERY_UPDATE._instance = null;
		}

		private QUERY_UPDATE()
		{
		}

		public override FDFSRequest GetRequest(params object[] paramList)
		{
			if ((int)paramList.Length != 2)
			{
				throw new FDFSException("param count is wrong");
			}
			QUERY_UPDATE queryupDate = new QUERY_UPDATE();
			string str = (string)paramList[0];
			string str1 = (string)paramList[1];
			if (str.Length > 16)
			{
				throw new FDFSException("GroupName is too long");
			}
			byte[] num = Util.StringToByte(str);
			byte[] numArray = Util.StringToByte(str1);
			int length = 16 + (int)numArray.Length;
			byte[] numArray1 = new byte[length];
			Array.Copy(num, 0, numArray1, 0, (int)num.Length);
			Array.Copy(numArray, 0, numArray1, 16, (int)numArray.Length);
			queryupDate.Body = numArray1;
			queryupDate.Header = new FDFSHeader((long)length, 103, 0);
			return queryupDate;
		}

		public class Response
		{
			public string GroupName;

			public string IPStr;

			public int Port;

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
			}
		}
	}
}