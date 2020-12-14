using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace connection_pool.Mysql
{
    public class Config
    {
        public Config(string username, string password, string hostname, string dbname)
        {
            this.Username = username;
            this.Password = password;
            this.Hostname = hostname;
            this.DbName = dbname;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Hostname { get; set; }

        public string DbName { get; set; }

        public int Port { get; set; } = 3306;

        public int PoolSize { get; set; } = 20;

        public PoolType PoolType { get; set; } = PoolType.Fixed;

        public string ConnectionString
        {
            get
            {
                return $"server={Hostname};port={Port};userid={Username};password={Password};database={DbName}";
            }
        }

        public string Id
        {
            get
            {
                return Username + "_" + Password + "_" + Hostname + "_" + Port + "_" + PoolSize + "_" + PoolType;
            }
        }
    }
}
