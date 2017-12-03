using System.Text;

namespace FastDFS.Client
{
    internal class FDFSConfig
    {
        public static readonly Encoding Charset = Encoding.UTF8;
        public const int ConnectionLifeTime = 1800;
        public const int GetConnectionTimeout = 5;
        public const int StorageMaxConnection = 120;
        public const int TrackerMaxConnection = 60;
    }
}