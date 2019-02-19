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
            var groupNameByteCount = Util.StringByteCount(groupName);
            if (groupNameByteCount > 16)
            {
                throw new FDFSException("groupName is too long");
            }
            var fileNameByteCount = Util.StringByteCount(fileName);
            int length = 16 + fileNameByteCount;
            deleteFile.SetBodyBuffer(length);
            Util.StringToByte(groupName, deleteFile.BodyBuffer, 0, groupNameByteCount);
            Util.StringToByte(fileName, deleteFile.BodyBuffer, 16, fileNameByteCount);
            deleteFile.Header = new FDFSHeader(length, FDFSConstants.STORAGE_PROTO_CMD_DELETE_FILE, 0);
            return deleteFile;
        }
    }
}