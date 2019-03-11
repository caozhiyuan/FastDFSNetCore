using System;
using System.Net;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    public class FastDFSClient
    {
        public static Task<FDFSFileInfo> GetFileInfoAsync(StorageNode storageNode, string fileName)
        {
            return QUERY_FILE_INFO.Instance.GetRequest(storageNode.EndPoint,
                    storageNode.GroupName,
                    fileName)
                .GetResponseAsync<FDFSFileInfo>();
        }

        public static async Task<StorageNode> GetStorageNodeAsync(string groupName)
        {
            var response = await QUERY_STORE_WITH_GROUP_ONE.Instance.GetRequest(groupName)
                .GetResponseAsync<QUERY_STORE_WITH_GROUP_ONE.Response>();

            var point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);
            return new StorageNode
            {
                GroupName = response.GroupName,
                EndPoint = point,
                StorePathIndex = response.StorePathIndex
            };
        }

        public static async Task RemoveFileAsync(string groupName, string fileName)
        {
            var response = await QUERY_UPDATE.Instance
                .GetRequest(groupName, fileName)
                .GetResponseAsync<QUERY_UPDATE.Response>();

            var point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);
            await DELETE_FILE.Instance
                .GetRequest(point, groupName, fileName)
                .GetResponseAsync<EmptyFDFSResponse>();
        }

        public static async Task<string> UploadFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            var response = await UPLOAD_FILE.Instance.GetRequest(storageNode,
                    fileExt, 
                    contentByte)
                .GetResponseAsync<UPLOAD_FILE.Response>();
            return response.FileName;
        }

        public static async Task<string> UploadAppenderFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            var response = await UPLOAD_APPEND_FILE.Instance.GetRequest(storageNode,
                    fileExt,
                    contentByte)
                .GetResponseAsync<UPLOAD_APPEND_FILE.Response>();
            return response.FileName;
        }

        public static async Task AppendFileAsync(string groupName, string fileName, byte[] contentByte)
        {
            var response = await QUERY_UPDATE.Instance
                .GetRequest(groupName, fileName)
                .GetResponseAsync<QUERY_UPDATE.Response>();

            var point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);
            await APPEND_FILE.Instance.GetRequest(point,
                    fileName,
                    contentByte)
                .GetResponseAsync<APPEND_FILE.Response>();
        }

        public static async Task<byte[]> DownloadFileAsync(StorageNode storageNode, string fileName,
            long offset = 0,
            long length = 0)
        {
            var response = await DOWNLOAD_FILE.Instance
                .GetRequest(storageNode,
                    fileName,
                    Tuple.Create(offset, length))
                .GetResponseAsync<DOWNLOAD_FILE.Response>();

            return response.Content;
        }
    }
}