using System;
using System.Net;

namespace FastDFS.Client
{
    internal class UPLOAD_FILE : FDFSRequest
    {
        // Fields
        private static UPLOAD_FILE _instance = null;

        // Methods
        private UPLOAD_FILE()
        {
        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 5)
            {
                throw new FDFSException("param count is wrong");
            }
            IPEndPoint endPoint = (IPEndPoint) paramList[0];
            byte num = (byte) paramList[1];
            int num2 = (int) paramList[2];
            string input = (string) paramList[3];
            byte[] sourceArray = (byte[]) paramList[4];
            byte[] destinationArray = new byte[6];
            byte[] buffer3 = Util.StringToByte(input);
            int length = buffer3.Length;
            if (length > 6)
            {
                length = 6;
            }
            Array.Copy(buffer3, 0, destinationArray, 0, length);
            UPLOAD_FILE uploadFile = new UPLOAD_FILE
            {
                ConnectionType = 1,
                EndPoint = endPoint
            };
            if (input.Length > 6)
            {
                throw new FDFSException("file ext is too long");
            }
            long num4 = 15 + sourceArray.Length;
            byte[] buffer4 = new byte[num4];
            buffer4[0] = num;
            byte[] buffer5 = Util.LongToBuffer((long) num2);
            Array.Copy(buffer5, 0, buffer4, 1, buffer5.Length);
            Array.Copy(destinationArray, 0, buffer4, 9, destinationArray.Length);
            Array.Copy(sourceArray, 0, buffer4, 15, sourceArray.Length);
            uploadFile.Body = buffer4;
            uploadFile.Header = new FDFSHeader(num4, 11, 0);
            return uploadFile;
        }

        // Properties
        public static UPLOAD_FILE Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UPLOAD_FILE();
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