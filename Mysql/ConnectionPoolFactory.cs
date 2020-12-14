using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace connection_pool.Mysql
{
    public enum PoolType
    {
        Cached,
        Fixed
    }

    public abstract class ConnectionPoolFactory
    {
        public static ConcurrentDictionary<PoolType, IConnectionPool> ConnectionPoolStrategy = new ConcurrentDictionary<PoolType, IConnectionPool>();

        public static IConnectionPool GetMySqlConnectionPool(Config config)
        {
            IConnectionPool pool;
            if (!ConnectionPoolStrategy.TryGetValue(config.PoolType, out pool))
            {
                pool = config.PoolType == PoolType.Cached ? (IConnectionPool)new CachedConnectionPool() : new FixedConnectionPool();
                ConnectionPoolStrategy.TryAdd(config.PoolType, pool);
            }

            return pool;
        }
    }
}
