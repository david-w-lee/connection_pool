using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace connection_pool.Mysql
{
    public class CachedConnectionPool : IConnectionPool
    {
        public ConcurrentDictionary<string, ConcurrentQueue<MySqlConnection>> ConnectionPool = new ConcurrentDictionary<string, ConcurrentQueue<MySqlConnection>>();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void DrainConnectionPool()
        {
            log.Debug("Draining connection pool");
            ConnectionPool.Clear();
        }

        public MySqlConnection GetMySqlConnection(Config config)
        {
            MySqlConnection mysqlConnection = null;
            if (config.PoolSize > 0)
            {
                ConcurrentQueue<MySqlConnection> queue;
                if (ConnectionPool.TryGetValue(config.Id, out queue))
                {
                    try
                    {
                        if (queue.TryDequeue(out mysqlConnection))
                        {
                            log.Debug("Reusing the existing mysqlConnection. Current Pool Size: " + queue.Count);
                            return mysqlConnection;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            mysqlConnection = new MySqlConnection(config.ConnectionString);

            mysqlConnection.Open();
            if (mysqlConnection.State == System.Data.ConnectionState.Open)
            {
                log.Debug("Creating a new mysqlConnection. Current Pool Size: 0");
                return mysqlConnection;
            }
            return null;
        }

        public void ReturnMySqlConnection(Config config, MySqlConnection connection)
        {
            if (config.PoolSize > 0)
            {
                ConcurrentQueue<MySqlConnection> queue;
                if (!ConnectionPool.TryGetValue(config.Id, out queue))
                {
                    queue = new ConcurrentQueue<MySqlConnection>();
                    ConnectionPool.TryAdd(config.Id, queue);
                }

                if (queue.Count < config.PoolSize)
                {
                    queue.Enqueue(connection);
                    log.Debug("Returning connection into the pool. Current Pool Size: " + queue.Count);
                }
                else
                {
                    log.Debug("Not returning connection into pool. Current Pool Size: " + queue.Count);
                    connection.Close();
                }
            }
        }
    }
}
