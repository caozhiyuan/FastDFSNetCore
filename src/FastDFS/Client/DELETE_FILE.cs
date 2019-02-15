using System;
using System.Net;

namespace FastDFS.Client
{
    internal class DELETE_FILE : FDFSRequest
    {
        public static readonly DELETE_FILE Instance = new DELETE_FILE();

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
            string groupName = (string) paramList[1];
            string fileName = (string) paramList[2];
            DELETE_FILE deleteFile = new DELETE_FILE
            {
                ConnectionType = FDFSConnectionType.StorageConnection,
                EndPoint = endPoint
            };
            if (groupName.Length > 0x10)
            {
                throw new FDFSException("groupName is too long");
            }
            var fileNameArray = Util.StringToByte(fileName);
            int length = 16 + fileNameArray.Length;
            deleteFile.SetBodyBuffer(length);
            var groupNameArray = Util.StringToByte(groupName);
            groupNameArray.CopyTo(new Span<byte>(deleteFile.BodyBuffer, 0, groupNameArray.Length));
            fileNameArray.CopyTo(new Span<byte>(deleteFile.BodyBuffer, 16, fileNameArray.Length));
            deleteFile.Header = new FDFSHeader(length, 12, 0);
            return deleteFile;
        }
    }
}