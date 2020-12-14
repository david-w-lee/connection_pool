using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace connection_pool.Mysql
{
    public class FixedConnectionPool : IConnectionPool
    {
        public ConcurrentDictionary<string, BlockingCollection<MySqlConnection>> ConnectionPool = new ConcurrentDictionary<string, BlockingCollection<MySqlConnection>>();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MySqlConnection GetMySqlConnection(Config config)
        {
            MySqlConnection mysqlConnection = null;
            if (config.PoolSize > 0)
            {
                BlockingCollection<MySqlConnection> queue;
                if (!ConnectionPool.TryGetValue(config.Id, out queue))
                {
                    queue = new BlockingCollection<MySqlConnection>();
                    ConnectionPool.TryAdd(config.Id, queue);

                    while (queue.Count < config.PoolSize)
                    {
                        mysqlConnection = new MySqlConnection(config.ConnectionString);
                        mysqlConnection.Open();
                        if (mysqlConnection.State == System.Data.ConnectionState.Open)
                        {
                            queue.Add(mysqlConnection);
                            log.Debug("Creating a new mysqlConnection. Current Pool Size: " + queue.Count);
                        }
                    }
                }

                mysqlConnection = queue.Take();
                return mysqlConnection;
            }

            return null;
        }

        public void ReturnMySqlConnection(Config config, MySqlConnection connection)
        {
            if (config.PoolSize > 0)
            {
                BlockingCollection<MySqlConnection> queue;
                if (ConnectionPool.TryGetValue(config.Id, out queue))
                {
                    queue.Add(connection);
                    log.Debug("Returning connection into the pool. Current Pool Size: " + queue.Count);
                }
                else
                {
                    connection.Close();
                }
            }

        }

        public void DrainConnectionPool()
        {
            log.Debug("Draining connection pool");
            ConnectionPool.Clear();
        }
    }
}
