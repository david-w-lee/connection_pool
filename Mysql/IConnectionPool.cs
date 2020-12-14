using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace connection_pool.Mysql
{
    public interface IConnectionPool
    {
        MySqlConnection GetMySqlConnection(Config config);

        void ReturnMySqlConnection(Config config, MySqlConnection connection);

        void DrainConnectionPool();
    }
}
