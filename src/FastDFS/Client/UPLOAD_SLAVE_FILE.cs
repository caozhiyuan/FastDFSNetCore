using System;
using System.Net;

namespace FastDFS.Client
{
    internal class UPLOAD_SLAVE_FILE : FDFSRequest
    {
        // Fields
        private static UPLOAD_SLAVE_FILE _instance = null;

        // Methods
        private UPLOAD_SLAVE_FILE()
        {
        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 6)
            {
                throw new FDFSException("param count is wrong");
            }
            IPEndPoint endPoint = (IPEndPoint) paramList[0];
            int num = (int) paramList[1];
            string input = paramList[2].ToString();
            string str2 = (string) paramList[3];
            string str3 = (string) paramList[4];
            byte[] sourceArray = (byte[]) paramList[5];
            byte[] buffer4 = Util.StringToByte(input);
            byte[] destinationArray = new byte[6];
            byte[] buffer6 = Util.StringToByte(str3);
            int length = buffer6.Length;
            if (length > 6)
            {
                length = 6;
            }
            Array.Copy(buffer6, 0, destinationArray, 0, length);
            UPLOAD_SLAVE_FILE uploadSlaveFile = new UPLOAD_SLAVE_FILE
            {
                ConnectionType = 1,
                EndPoint = endPoint
            };
            if (str3.Length > 6)
            {
                throw new FDFSException("file ext is too long");
            }
            byte[] buffer2 = new byte[0x10];
            long num4 = (((buffer2.Length + 0x10) + 6) + buffer4.Length) + sourceArray.Length;
            byte[] buffer7 = new byte[num4];
            byte[] buffer3 = Util.LongToBuffer((long) input.Length);
            int destinationIndex = buffer3.Length;
            Array.Copy(buffer3, 0, buffer7, 0, buffer3.Length);
            byte[] buffer8 = Util.LongToBuffer((long) num);
            Array.Copy(buffer8, 0, buffer7, destinationIndex, buffer8.Length);
            destinationIndex = buffer2.Length;
            byte[] buffer9 = new byte[0x10];
            byte[] buffer10 = Util.StringToByte(str2);
            int num5 = buffer10.Length;
            if (num5 > 0x10)
            {
                num5 = 0x10;
            }
            if (num5 > 0)
            {
                Array.Copy(buffer10, 0, buffer9, 0, num5);
            }
            Array.Copy(buffer9, 0, buffer7, destinationIndex, buffer9.Length);
            destinationIndex += buffer9.Length;
            Array.Copy(destinationArray, 0, buffer7, destinationIndex, destinationArray.Length);
            destinationIndex += destinationArray.Length;
            Array.Copy(buffer4, 0, buffer7, destinationIndex, buffer4.Length);
            destinationIndex += buffer4.Length;
            Array.Copy(sourceArray, 0, buffer7, destinationIndex, sourceArray.Length);
            uploadSlaveFile.Body = buffer7;
            uploadSlaveFile.Header = new FDFSHeader(num4, 0x15, 0);
            return uploadSlaveFile;
        }

        // Properties
        public static UPLOAD_SLAVE_FILE Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UPLOAD_SLAVE_FILE();
                }
                return _instance;
            }
        }

        // Nested Types
        public class Response
        {
            // Fields
            public string FileName;
            public string GroupName;

            // Methods
            public Response(byte[] responseBody)
            {
                byte[] destinationArray = new byte[0x10];
                Array.Copy(responseBody, destinationArray, 0x10);
                this.GroupName = Util.ByteToString(destinationArray).TrimEnd(new char[1]);
                byte[] buffer2 = new byte[responseBody.Length - 0x10];
                Array.Copy(responseBody, 0x10, buffer2, 0, buffer2.Length);
                this.FileName = Util.ByteToString(buffer2).TrimEnd(new char[1]);
            }
        }
    }
}