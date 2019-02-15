using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FastDFS.Client;

namespace FastDFS.Test
{
    internal class Program
    {
        const string StorageLink = "http://192.168.238.128/group1/";

        static void Main(string[] args)
        {
            List<IPEndPoint> pEndPoints = new List<IPEndPoint>()
            {
                new IPEndPoint(IPAddress.Parse("192.168.238.128"), 22122)
            };
            ConnectionManager.Initialize(pEndPoints);

            while (true)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                AsyncTest().Wait();

                sw.Stop();
                Console.WriteLine("AsyncTest " + sw.ElapsedMilliseconds);

                Console.ReadKey();

                sw.Start();

                SyncTest();

                sw.Stop();

                Console.WriteLine("SyncTest " + sw.ElapsedMilliseconds);

                Console.ReadKey();

                TestGetFileInfo().Wait();

                Console.ReadKey();

                ParallelTest();

                Console.ReadKey();
            }
        }

        /// <summary>
        /// ParallelTest
        /// </summary>
        /// <returns></returns>
        private static void ParallelTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var by = GetFileBytes("testimage/1.jpg");
            const int c = 500;
            CountdownEvent k = new CountdownEvent(c);
            Parallel.For(0, c, (i) =>
            {
                var task = UploadAsync2(StorageLink, by);
                task.ContinueWith(n =>
                {
                    if (n.IsFaulted)
                    {
                        Console.Write("E");
                    }
                    k.Signal(1);
                });
            });

            k.Wait();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// UploadAsync2
        /// </summary>
        /// <param name="storageLink"></param>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        private static async Task UploadAsync2(string storageLink, byte[] fileBytes)
        {
            StorageNode storageNode = await FastDFSClient.GetStorageNodeAsync("group1");
            var str = await FastDFSClient.UploadFileAsync(storageNode, fileBytes, "jpg");
            Console.WriteLine(storageLink + str);

            await FastDFSClient.RemoveFileAsync("group1", str);
            Console.WriteLine("FastDFSClient.RemoveFile" + str);
        }

        /// <summary>
        /// UploadAsync
        /// </summary>
        /// <param name="storageLink"></param>
        /// <returns></returns>
        private static async Task UploadAsync(string storageLink)
        {
            StorageNode storageNode = await FastDFSClient.GetStorageNodeAsync("group1");
            string[] files = Directory.GetFiles("testimage", "*.jpg");
            string[] strArrays = files;
            for (int i = 0; i < strArrays.Length; i++)
            {
                string str1 = strArrays[i];
                var numArray = GetFileBytes(str1);
                var str = await FastDFSClient.UploadFileAsync(storageNode, numArray, "jpg");
                Console.WriteLine(storageLink + str);
                await FastDFSClient.RemoveFileAsync("group1", str);
                Console.WriteLine("FastDFSClient.RemoveFile" + str);
            }
        }

        private static readonly object Locker = new object();

        /// <summary>
        /// GetFileBytes
        /// </summary>
        /// <param name="str1"></param>
        /// <returns></returns>
        private static byte[] GetFileBytes(string str1)
        {
            lock (Locker)
            {
                var fileStream = new FileStream(str1, FileMode.Open);
                var binaryReader = new BinaryReader(fileStream);
                byte[] numArray;
                try
                {
                    numArray = binaryReader.ReadBytes((int)fileStream.Length);
                }
                finally
                {
                    binaryReader.Dispose();
                }
                return numArray;
            }
        }

        /// <summary>
        /// AsyncTest
        /// </summary>
        /// <returns></returns>
        private static async Task AsyncTest()
        {
            await UploadAsync(StorageLink);
        }

        /// <summary>
        /// SyncTest
        /// </summary>
        /// <returns></returns>
        private static void SyncTest()
        {
            StorageNode storageNode = FastDFSClient.GetStorageNodeAsync("group1").GetAwaiter().GetResult();
            string[] files = Directory.GetFiles("testimage", "*.jpg");
            string[] strArrays = files;
            for (int i = 0; i < strArrays.Length; i++)
            {
                string str1 = strArrays[i];
                var fileStream = new FileStream(str1, FileMode.Open);
                var binaryReader = new BinaryReader(fileStream);
                byte[] numArray;
                try
                {
                    numArray = binaryReader.ReadBytes((int)fileStream.Length);
                }
                finally
                {
                    binaryReader.Dispose();
                }
                var str = FastDFSClient.UploadFileAsync(storageNode, numArray, "jpg").GetAwaiter().GetResult();
                Console.WriteLine(StorageLink + str);
                FastDFSClient.RemoveFileAsync("group1", str).GetAwaiter().GetResult(); ;
                Console.WriteLine("FastDFSClient.RemoveFile" + str);
            }
        }

        /// <summary>
        /// TestGetFileInfo
        /// </summary>
	    private static async Task TestGetFileInfo()
        {
            StorageNode storageNode = await FastDFSClient.GetStorageNodeAsync("group1");
            var str = "M00/0E/82/CgE-EFsTqKmAEHO7AADMG7XS9Fc7491.jpg";
            FDFSFileInfo fileInfo = await FastDFSClient.GetFileInfoAsync(storageNode, str);
            if (fileInfo == null)
            {
                string[] files = Directory.GetFiles("testimage", "*.jpg");
                string[] strArrays = files;
                var numArray = GetFileBytes(strArrays[0]);
                str = await FastDFSClient.UploadFileAsync(storageNode, numArray, "jpg");
                fileInfo = await FastDFSClient.GetFileInfoAsync(storageNode, str);
                if (fileInfo == null)
                {
                    Console.WriteLine($"GetFileInfoAsync Fail, path: {str}");
                    return;
                }
            }

            Console.WriteLine("FileName:{0}", str);
            Console.WriteLine("FileSize:{0}", fileInfo.FileSize);
            Console.WriteLine("CreateTime:{0}", fileInfo.CreateTime);
            Console.WriteLine("Crc32:{0}", fileInfo.Crc32);

            Console.WriteLine("Complete");
        }
    }
}