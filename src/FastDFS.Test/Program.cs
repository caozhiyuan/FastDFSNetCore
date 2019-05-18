using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastDFS.Client;

namespace FastDFS.Test
{
    internal class Program
    {
        const string StorageLink = "http://192.168.197.128/group1/";
        private static readonly HttpClient HttpClient = new HttpClient();

        static void Main(string[] args)
        {
            List<IPEndPoint> pEndPoints = new List<IPEndPoint>()
            {
                new IPEndPoint(IPAddress.Parse("192.168.197.128"), 22122)
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

                UploadAppendFile().Wait();

                DownLoadFile().Wait();

                Console.ReadKey();

                ParallelTest();

                Console.ReadKey();

                sw.Start();

                BigFileUploadDownLoad().Wait();

                sw.Stop();
                Console.WriteLine("BigFileUploadDownLoad 100M " + sw.ElapsedMilliseconds);

                sw.Start();

                BigFileAppendUploadDownLoad().Wait();

                sw.Stop();
                Console.WriteLine("BigFileAppendUploadDownLoad 100M " + sw.ElapsedMilliseconds);

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

        private static async Task UploadAppendFile()
        {
            var testBytes = Encoding.UTF8.GetBytes("123456789");
            StorageNode storageNode = await FastDFSClient.GetStorageNodeAsync("group1");
            var filename = await FastDFSClient.UploadAppenderFileAsync(storageNode, testBytes.Take(6).ToArray(), "txt");
            FDFSFileInfo fileInfo = await FastDFSClient.GetFileInfoAsync(storageNode, filename);
            if (fileInfo == null)
            {
                Console.WriteLine($"GetFileInfoAsync Fail, path: {filename}");
                return;
            }

            Console.WriteLine("FileName:{0}", filename);
            Console.WriteLine("FileSize:{0}", fileInfo.FileSize);
            Console.WriteLine("CreateTime:{0}", fileInfo.CreateTime);
            Console.WriteLine("Crc32:{0}", fileInfo.Crc32);

            var appendBytes = testBytes.Skip((int) fileInfo.FileSize).ToArray();
            await FastDFSClient.AppendFileAsync("group1", filename, appendBytes);

            var test = await HttpClient.GetByteArrayAsync(StorageLink + filename);
            if (Encoding.UTF8.GetString(test) == Encoding.UTF8.GetString(testBytes))
            {
                Console.WriteLine($"UploadAppendFile Success");
            }
            else
            {
                Console.WriteLine($"UploadAppendFile Fail : Bytes Diff ");
            }
            await FastDFSClient.RemoveFileAsync("group1", filename);

        }

        private static async Task DownLoadFile()
        {
            var testBytes = Encoding.UTF8.GetBytes("123456789");
            StorageNode storageNode = await FastDFSClient.GetStorageNodeAsync("group1");
            var filename = await FastDFSClient.UploadFileAsync(storageNode, testBytes, "txt");

            var bytes = await FastDFSClient.DownloadFileAsync(storageNode, filename);
            if (bytes == null)
            {
                Console.WriteLine($"DownLoadFile Fail : Bytes null ");
            }
            if (Encoding.UTF8.GetString(bytes) == Encoding.UTF8.GetString(testBytes))
            {
                Console.WriteLine($"DownLoadFile Success");
            }
            else
            {
                Console.WriteLine($"DownLoadFile Fail : Bytes Diff ");
            }
        }

        protected static async Task BigFileUploadDownLoad()
        {
            var temp = Enumerable.Repeat((byte) 99, 1024 * 1024 * 100);
            var testBytes = temp.ToArray();
            StorageNode storageNode = await FastDFSClient.GetStorageNodeAsync("group1");

            var filename = await FastDFSClient.UploadFileAsync(storageNode, testBytes, "txt");

            using (var fileStream = File.OpenWrite("c:\\fastdfs_test.txt"))
            {
                await FastDFSClient.DownloadFileAsync(storageNode, filename, new StreamDownloadCallback(fileStream));
            }

            await FastDFSClient.RemoveFileAsync("group1", filename);

            temp = null;
            testBytes = null;
        }

        protected static async Task BigFileAppendUploadDownLoad()
        {
            var temp = Enumerable.Repeat((byte)99, 1024 * 1024 * 100);
            var testBytes = temp.ToArray();
            StorageNode storageNode = await FastDFSClient.GetStorageNodeAsync("group1");


            var filename = await FastDFSClient.UploadAppenderFileAsync(storageNode, testBytes.Take(1024 * 1024 * 2).ToArray(), "txt");

            for (int i = 0; i < 49; i++)
            {
                FDFSFileInfo fileInfo = await FastDFSClient.GetFileInfoAsync(storageNode, filename);
                var appendBytes = testBytes.Skip((int)fileInfo.FileSize).Take(1024 * 1024 * 2).ToArray();
                await FastDFSClient.AppendFileAsync("group1", filename, appendBytes);
            }

            using (var fileStream = File.OpenWrite("c:\\fastdfs_test.txt"))
            {
                for (int i = 0; i < 50; i++)
                {
                    var buffer = await FastDFSClient.DownloadFileAsync(storageNode, filename,
                        1024 * 1024 * 2 * i,
                        1024 * 1024 * 2);
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }

            await FastDFSClient.RemoveFileAsync("group1", filename);

            temp = null;
            testBytes = null;
        }
    }
}