using System.Net;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    public class FastDFSClient
    {
        public static FDFSFileInfo GetFileInfo(StorageNode storageNode, string fileName)
        {
            return
                new FDFSFileInfo(
                    QUERY_FILE_INFO.Instance.GetRequest(new object[]
                    {storageNode.EndPoint, storageNode.GroupName, fileName}).GetResponse());
        }

        public static async Task<FDFSFileInfo> GetFileInfoAsync(StorageNode storageNode, string fileName)
        {
            return
                new FDFSFileInfo(
                    await QUERY_FILE_INFO.Instance.GetRequest(new object[]
                        {storageNode.EndPoint, storageNode.GroupName, fileName}).GetResponseAsync());
        }

        public static StorageNode GetStorageNode(string groupName)
        {
            QUERY_STORE_WITH_GROUP_ONE.Response response =
                new QUERY_STORE_WITH_GROUP_ONE.Response(
                    QUERY_STORE_WITH_GROUP_ONE.Instance.GetRequest(new object[] {groupName}).GetResponse());
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);
            return new StorageNode
            {
                GroupName = response.GroupName,
                EndPoint = point,
                StorePathIndex = response.StorePathIndex
            };
        }

        public static async Task<StorageNode> GetStorageNodeAsync(string groupName)
        {
            var responseBuffer = await QUERY_STORE_WITH_GROUP_ONE.Instance.GetRequest(new object[] {groupName}).GetResponseAsync();
            var response = new QUERY_STORE_WITH_GROUP_ONE.Response(responseBuffer);
            var point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);
            return new StorageNode
            {
                GroupName = response.GroupName,
                EndPoint = point,
                StorePathIndex = response.StorePathIndex
            };
        }

        public static void RemoveFile(string groupName, string fileName)
        {
            QUERY_UPDATE.Response response =
                new QUERY_UPDATE.Response(
                    QUERY_UPDATE.Instance.GetRequest(new object[] {groupName, fileName}).GetResponse());
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);
            DELETE_FILE.Instance.GetRequest(new object[] {point, groupName, fileName}).GetResponse();
        }

        public static async Task RemoveFileAsync(string groupName, string fileName)
        {
            var buffer = await QUERY_UPDATE.Instance.GetRequest(new object[] {groupName, fileName}).GetResponseAsync();
            var response = new QUERY_UPDATE.Response(buffer);
            var point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);
            await DELETE_FILE.Instance.GetRequest(new object[] { point, groupName, fileName }).GetResponseAsync();
        }

        public static string UploadAppenderFile(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            UPLOAD_APPEND_FILE.Response response =
                new UPLOAD_APPEND_FILE.Response(
                    UPLOAD_APPEND_FILE.Instance.GetRequest(new object[]
                    {storageNode.EndPoint, storageNode.StorePathIndex, contentByte.Length, fileExt, contentByte})
                        .GetResponse());
            return response.FileName;
        }

        public static async Task<string> UploadAppenderFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            UPLOAD_APPEND_FILE.Response response =
                new UPLOAD_APPEND_FILE.Response(
                    await UPLOAD_APPEND_FILE.Instance.GetRequest(new object[]
                            {storageNode.EndPoint, storageNode.StorePathIndex, contentByte.Length, fileExt, contentByte})
                        .GetResponseAsync());
            return response.FileName;
        }

        public static string UploadFile(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            UPLOAD_FILE.Response response =
                new UPLOAD_FILE.Response(
                    UPLOAD_FILE.Instance.GetRequest(new object[]
                    {storageNode.EndPoint, storageNode.StorePathIndex, contentByte.Length, fileExt, contentByte})
                        .GetResponse());
            return response.FileName;
        }

        public static async Task<string> UploadFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            var responseBuffer = await UPLOAD_FILE.Instance.GetRequest(new object[]
            {
                storageNode.EndPoint,
                storageNode.StorePathIndex,
                contentByte.Length,
                fileExt,
                contentByte
            }).GetResponseAsync();
            var response = new UPLOAD_FILE.Response(responseBuffer);
            return response.FileName;
        }
    }
}