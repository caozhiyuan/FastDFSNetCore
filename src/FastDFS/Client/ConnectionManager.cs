using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    public class ConnectionManager
    {
        private static List<EndPoint> _listTrackers = new List<EndPoint>();
        private static readonly Dictionary<EndPoint, Pool> TrackerPools = new Dictionary<EndPoint, Pool>();
        private static readonly ConcurrentDictionary<EndPoint, Pool> StorePools = new ConcurrentDictionary<EndPoint, Pool>();

        internal static Task<Connection> GetStorageConnectionAsync(EndPoint endPoint)
        {
            return StorePools.GetOrAdd(endPoint, (ep) => new Pool(ep, FDFSConfig.StorageMaxConnection)).GetConnectionAsync();
        }

        internal static Task<Connection> GetTrackerConnectionAsync()
        {
            int num = new Random().Next(TrackerPools.Count);
            Pool pool = TrackerPools[_listTrackers[num]];
            return pool.GetConnectionAsync();
        }

        public static bool Initialize(IEnumerable<EndPoint> trackers)
        {
            foreach (EndPoint point in trackers)
            {
                if (!TrackerPools.ContainsKey(point))
                {
                    TrackerPools.Add(point, new Pool(point, FDFSConfig.TrackerMaxConnection));
                    _listTrackers.Add(point);
                }
            }
            return true;
        }
    }
}