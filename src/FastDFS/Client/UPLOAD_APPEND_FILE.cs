using System;
using System.Net;

namespace FastDFS.Client
{
    internal class UPLOAD_APPEND_FILE : FDFSRequest
	{
		private static UPLOAD_APPEND_FILE _instance;

		public static UPLOAD_APPEND_FILE Instance
		{
			get
			{
				if (UPLOAD_APPEND_FILE._instance == null)
				{
					UPLOAD_APPEND_FILE._instance = new UPLOAD_APPEND_FILE();
				}
				return UPLOAD_APPEND_FILE._instance;
			}
		}

		static UPLOAD_APPEND_FILE()
		{
			UPLOAD_APPEND_FILE._instance = null;
		}

		private UPLOAD_APPEND_FILE()
		{
		}

		public override FDFSRequest GetRequest(params object[] paramList)
		{
			if ((int)paramList.Length != 5)
			{
				throw new FDFSException("param count is wrong");
			}
			IPEndPoint pEndPoint = (IPEndPoint)paramList[0];
			byte num = (byte)paramList[1];
			int num1 = (int)paramList[2];
			string str = (string)paramList[3];
			byte[] numArray = (byte[])paramList[4];
			byte[] numArray1 = new byte[6];
			byte[] num2 = Util.StringToByte(str);
			int length = (int)num2.Length;
			if (length > 6)
			{
				length = 6;
			}
			Array.Copy(num2, 0, numArray1, 0, length);
			UPLOAD_APPEND_FILE uploadAppendFile = new UPLOAD_APPEND_FILE()
			{
			    ConnectionType = 1,
			    EndPoint = pEndPoint
            };
			if (str.Length > 6)
			{
				throw new FDFSException("file ext is too long");
			}
			long length1 = 15 + numArray.Length;
            byte[] numArray2 = new byte[length1];
			numArray2[0] = num;
			byte[] buffer = Util.LongToBuffer((long)num1);
			Array.Copy(buffer, 0, numArray2, 1, (int)buffer.Length);
			Array.Copy(numArray1, 0, numArray2, 9, (int)numArray1.Length);
			Array.Copy(numArray, 0, numArray2, 15, (int)numArray.Length);
			uploadAppendFile.Body = numArray2;
			uploadAppendFile.Header = new FDFSHeader(length1, 23, 0);
			return uploadAppendFile;
		}

		public class Response
		{
			public string GroupName;

			public string FileName;

			public Response(byte[] responseBody)
			{
				byte[] numArray = new byte[16];
				Array.Copy(responseBody, numArray, 16);
				this.GroupName = Util.ByteToString(numArray).TrimEnd(new char[1]);
				byte[] numArray1 = new byte[(int)responseBody.Length - 16];
				Array.Copy(responseBody, 16, numArray1, 0, (int)numArray1.Length);
				this.FileName = Util.ByteToString(numArray1).TrimEnd(new char[1]);
			}
		}
	}
}