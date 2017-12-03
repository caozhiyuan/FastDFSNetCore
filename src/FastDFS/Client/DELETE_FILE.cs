using System;
using System.Net;

namespace FastDFS.Client
{
    internal class DELETE_FILE : FDFSRequest
    {
        // Fields
        private static DELETE_FILE _instance = null;

        // Methods
        private DELETE_FILE()
        {
        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
            {
                throw new FDFSException("param count is wrong");
            }
            IPEndPoint endPoint = (IPEndPoint) paramList[0];
            string input = (string) paramList[1];
            string str2 = (string) paramList[2];
            DELETE_FILE deleteFile = new DELETE_FILE
            {
                ConnectionType = 1,
                EndPoint = endPoint
            };
            if (input.Length > 0x10)
            {
                throw new FDFSException("groupName is too long");
            }
            long length = 0x10 + str2.Length;
            byte[] destinationArray = new byte[length];
            byte[] sourceArray = Util.StringToByte(input);
            byte[] buffer3 = Util.StringToByte(str2);
            Array.Copy(sourceArray, 0, destinationArray, 0, sourceArray.Length);
            Array.Copy(buffer3, 0, destinationArray, 0x10, buffer3.Length);
            deleteFile.Body = destinationArray;
            deleteFile.Header = new FDFSHeader(length, 12, 0);
            return deleteFile;
        }

        // Properties
        public static DELETE_FILE Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DELETE_FILE();
                }
                return _instance;
            }
        }

        // Nested Types
        public class Response
        {
            // Methods
            public Response(byte[] responseBody)
            {
            }
        }
    }
}