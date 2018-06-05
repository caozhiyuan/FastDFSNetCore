using System.Net;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    public class FastDFSClient
    {
        public static async Task<FDFSFileInfo> GetFileInfoAsync(StorageNode storageNode, string fileName)
        {
            try
            {
                var req = QUERY_FILE_INFO.Instance.GetRequest(storageNode.EndPoint,
                    storageNode.GroupName,
                    fileName);

                return new FDFSFileInfo(await req.GetResponseAsync());
            }
            catch (FDFSStatusException)
            {
                return null;
            }
        }

        public static async Task<StorageNode> GetStorageNodeAsync(string groupName)
        {
            var responseBuffer = await QUERY_STORE_WITH_GROUP_ONE.Instance.GetRequest(groupName).GetResponseAsync();
            var response = new QUERY_STORE_WITH_GROUP_ONE.Response(responseBuffer);
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
            var buffer = await QUERY_UPDATE.Instance.GetRequest(groupName, fileName).GetResponseAsync();
            var response = new QUERY_UPDATE.Response(buffer);
            var point = new IPEndPoint(IPAddress.Parse(response.IPStr), response.Port);
            await DELETE_FILE.Instance.GetRequest(point, groupName, fileName).GetResponseAsync();
        }

        public static async Task<string> UploadFileAsync(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            var req = UPLOAD_FILE.Instance.GetRequest(
                storageNode.EndPoint,
                storageNode.StorePathIndex,
                contentByte.Length,
                fileExt,
                contentByte);
            var response = new UPLOAD_FILE.Response(await req.GetResponseAsync());
            return response.FileName;
        }
    }
}