using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    internal class Pool
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IPEndPoint _endPoint;
        private readonly Stack<Connection> _idle;
        private readonly List<Connection> _inUse;
        private readonly int _maxConnection;
        private readonly object _locker = new object();

        public Pool(IPEndPoint endPoint, int maxConnection)
        {
            this._semaphoreSlim = new SemaphoreSlim(maxConnection);
            this._inUse = new List<Connection>(maxConnection);
            this._idle = new Stack<Connection>(maxConnection);
            this._maxConnection = maxConnection;
            this._endPoint = endPoint;

        }

        public void CloseConnection(Connection conn)
        {
            conn.InUse = false;
            lock (_locker)
            {
                this._inUse.Remove(conn);
            }
            lock (_locker)
            {
                this._idle.Push(conn);
            }
            this._semaphoreSlim.Release();
        }

        public async Task<Connection> GetConnectionAsync()
        {
            int millisecondsTimeout = FDFSConfig.GetConnectionTimeout * 1000;
            while (true)
            {
                if (!await this._semaphoreSlim.WaitAsync(millisecondsTimeout))
                {
                    break;
                }

                var pooledConnection = this.GetPooledConnection();
                if (pooledConnection != null)
                {
                    return pooledConnection;
                }
            }
            throw new FDFSException("Get CanUse Connection Time Out");
        }

        private Connection GetPooledConnection()
        {
            Connection item = null;
            lock (_locker)
            {
                if (this._idle.Count > 0)
                {
                    item = this._idle.Pop();
                }
            }
            lock (_locker)
            {
                if (this._inUse.Count == this._maxConnection)
                {
                    return null;
                }
                if (item == null)
                {
                    item = new Connection(_endPoint)
                    {
                        Pool = this
                    };
                }
                this._inUse.Add(item);
            }
            return item;
        }
    }
}