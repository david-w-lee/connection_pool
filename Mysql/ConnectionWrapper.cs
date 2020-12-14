using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace connection_pool.Mysql
{
    public class ConnectionWrapper : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Config mySqlConfig;
        public MySqlConnection mySqlConnection;
        private IConnectionPool mySqlConnectionPool;
        public ConnectionWrapper(Config mySqlConfig)
        {
            this.mySqlConfig = mySqlConfig;
            this.mySqlConnectionPool = ConnectionPoolFactory.GetMySqlConnectionPool(mySqlConfig);
            this.mySqlConnection = mySqlConnectionPool.GetMySqlConnection(mySqlConfig);
        }

        public List<User> GetUsers(string prefix)
        {
            List<string> res = new List<string>();
            var sql = $"SELECT id, first_name, last_name FROM users WHERE first_name LIKE '{prefix}%'";
            using var cmd = new MySqlCommand(sql, mySqlConnection);

            using MySqlDataReader rdr = cmd.ExecuteReader();

            List<User> list = new List<User>();
            while (rdr.Read())
            {
                var user = new User();
                user.Id = rdr.GetInt32(0);
                user.FirstName = rdr.GetString(1);
                user.LastName = rdr.GetString(2);
                list.Add(user);
                log.Debug($"{user.Id} {user.FirstName} {user.LastName}");
            }

            return list;
        }

        public void AddUser(string firstname, string lastname)
        {
            var sql = "INSERT INTO users ( first_name, last_name ) VALUES (@first_name, @last_name)";
            using var cmd = new MySqlCommand(sql, mySqlConnection);

            cmd.Parameters.AddWithValue("@first_name", firstname);
            cmd.Parameters.AddWithValue("@last_name", lastname);
            cmd.Prepare();

            int res = cmd.ExecuteNonQuery();

            log.Debug($"DB Result : {res}");
        }

        public void Dispose()
        {
            mySqlConnectionPool.ReturnMySqlConnection(mySqlConfig, mySqlConnection);
        }
    }
}
