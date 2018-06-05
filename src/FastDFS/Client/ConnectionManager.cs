using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    public class ConnectionManager
    {
        private static List<IPEndPoint> _listTrackers = new List<IPEndPoint>();
        private static readonly Dictionary<IPEndPoint, Pool> TrackerPools = new Dictionary<IPEndPoint, Pool>();
        private static readonly ConcurrentDictionary<IPEndPoint, Pool> StorePools = new ConcurrentDictionary<IPEndPoint, Pool>();

        internal static Task<Connection> GetStorageConnectionAsync(IPEndPoint endPoint)
        {
            return StorePools.GetOrAdd(endPoint, (ep) => new Pool(ep, FDFSConfig.StorageMaxConnection)).GetConnectionAsync();
        }

        internal static Task<Connection> GetTrackerConnectionAsync()
        {
            int num = new Random().Next(TrackerPools.Count);
            Pool pool = TrackerPools[_listTrackers[num]];
            return pool.GetConnectionAsync();
        }

        public static bool Initialize(List<IPEndPoint> trackers)
        {
            foreach (IPEndPoint point in trackers)
            {
                if (!TrackerPools.ContainsKey(point))
                {
                    TrackerPools.Add(point, new Pool(point, FDFSConfig.TrackerMaxConnection));
                }
            }
            _listTrackers = trackers;
            return true;
        }
    }
}