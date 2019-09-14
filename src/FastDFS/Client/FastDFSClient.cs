using System;
using System.IO;
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

        public static Task<StorageNode> GetStorageNodeAsync()
        {
            return GetStorageNodeAsync(null);
        }

        public static async Task<StorageNode> GetStorageNodeAsync(string groupName)
        {
            var request = string.IsNullOrWhiteSpace(groupName) ?
                            QUERY_STORE_WITHOUT_GROUP_ONE.Instance.GetRequest()
                            : QUERY_STORE_WITH_GROUP_ONE.Instance.GetRequest(groupName);
            var response = await request.GetResponseAsync<QUERY_STORE_WITH_GROUP_ONE.Response>();

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

        public static async Task<string> UploadFileAsync(StorageNode storageNode, Stream contentStream, string fileExt)
        {
            var response = await UPLOAD_FILE.Instance.GetRequest(storageNode,
                    fileExt,
                    contentStream)
                .GetResponseAsync<UPLOAD_FILE.Response>();
            return response.FileName;
        }

        public static async Task<string> UploadFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            using (var contentStream = new MemoryStream(contentByte, false))
            {
                return await UploadFileAsync(storageNode, contentStream, fileExt);
            }
        }

        public static async Task<string> UploadAppenderFileAsync(StorageNode storageNode, Stream contentStream, string fileExt)
        {
            var response = await UPLOAD_APPEND_FILE.Instance.GetRequest(storageNode,
                    fileExt,
                    contentStream)
                .GetResponseAsync<UPLOAD_APPEND_FILE.Response>();
            return response.FileName;
        }

        public static async Task<string> UploadAppenderFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            using (var contentStream = new MemoryStream(contentByte, false))
            {            
                return await UploadAppenderFileAsync(storageNode,contentStream,fileExt);
            }
        }

        public static async Task AppendFileAsync(string groupName, string fileName, Stream contentStream)
        {
            var response = await QUERY_UPDATE.Instance
                .GetRequest(groupName, fileName)
                .GetResponseAsync<QUERY_UPDATE.Response>();

            var point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);

            await APPEND_FILE.Instance.GetRequest(point,
                    fileName,
                    contentStream)
                .GetResponseAsync<APPEND_FILE.Response>();
        }

        public static async Task AppendFileAsync(string groupName, string fileName, byte[] contentByte)
        {
            using (var contentStream = new MemoryStream(contentByte, false))
            {
                await AppendFileAsync(groupName, fileName, contentStream);
            }
        }

        public static async Task<byte[]> DownloadFileAsync(StorageNode storageNode, string fileName,
            long offset = 0,
            long length = 0)
        {
            using (var memoryStream = new MemoryStream(length > 0 ? (int) length : 0))
            {
                var downloadStream = new StreamDownloadCallback(memoryStream);
                await DownloadFileAsync(storageNode, fileName, downloadStream, offset, length);
                return memoryStream.ToArray();
            }
        }

        public static async Task DownloadFileAsync(StorageNode storageNode, string fileName,
            IDownloadCallback downloadCallback,
            long offset = 0,
            long length = 0)
        {
            await DOWNLOAD_FILE.Instance
                .GetRequest(storageNode,
                    fileName,
                    Tuple.Create(offset, length),
                    downloadCallback)
                .GetResponseAsync<DOWNLOAD_FILE.Response>();
        }
    }
}