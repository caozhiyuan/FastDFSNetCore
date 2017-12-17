using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FastDFS.Client;

namespace FastDFS.Test
{
    public class FDFSConfig
    {
        public string TackerIp { get; set; }

        public string TackerPort { get; set; }

        public string StorageGroup { get; set; }

        public string StorageServerLink { get; set; }
    }

    public static class FDFSHelper
    {
        /// <summary>
        /// GetFastDfsConfig
        /// </summary>
        /// <returns></returns>
        private static Task<FDFSConfig> GetFastDfsConfig()
        {
            //mock 
            //get from app.config or zk
            return Task.FromResult(new FDFSConfig
            {
                TackerIp = "",
                TackerPort = "23000",
                StorageGroup = "",
                StorageServerLink = ""
            });
        }

        /// <summary>
        /// 静态锁
        /// </summary>
        private static readonly object InitializeLock = new object();

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="contentBytes">比特数组</param>
        /// <param name="imageType">图片类型</param>
        /// <returns>图片地址</returns>
        public static async Task<string> FastDFSUploadFile(byte[] contentBytes, string imageType)
        {
            if (contentBytes == null || contentBytes.Length == 0)
                throw new ArgumentNullException("contentBytes");

            if (string.IsNullOrEmpty(imageType))
                throw new ArgumentNullException("imageType");

            var config = await GetFastDfsConfig();

            EnsureConnectionInitialize(config);

            var group = config.StorageGroup;
            var storageNode = await FastDFSClient.GetStorageNodeAsync(group);
            string paths = await FastDFSClient.UploadFileAsync(storageNode, contentBytes, imageType);

            StringBuilder resultImageUrl = new StringBuilder();
            var storageLink = config.StorageServerLink;
            resultImageUrl.Append(storageLink);
            resultImageUrl.Append(paths);

            return resultImageUrl.ToString();
        }

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private static bool _isInitialize;

        /// <summary>
        /// 初始Connection
        /// </summary>
        private static void EnsureConnectionInitialize(FDFSConfig config)
        {
            if (!_isInitialize)
            {
                lock (InitializeLock)
                {
                    if (!_isInitialize)
                    {
                        var trackerIp = config.TackerIp;
                        int trackerPort = Convert.ToInt32(config.TackerPort);
                        List<IPEndPoint> trackerIPs = new List<IPEndPoint>();
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(trackerIp), trackerPort);
                        trackerIPs.Add(endPoint);
                        ConnectionManager.Initialize(trackerIPs);
                        _isInitialize = true;
                    }
                }
            }
        }
    }
}
