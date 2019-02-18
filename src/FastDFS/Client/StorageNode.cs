using System;
using System.Net;

namespace FastDFS.Client
{
    public class StorageNode
    {
        public string GroupName;

        public IPEndPoint EndPoint;

        public byte StorePathIndex;

        public StorageNode()
        {
        }
    }
}