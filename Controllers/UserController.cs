using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using connection_pool.Mysql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace connection_pool.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        public IEnumerable<User> Get()
        {
            Config config = new Config("root", "test", "localhost", "sampledb");
            using (var connection = new ConnectionWrapper(config))
            {
                return connection.GetUsers(GenerateString(1));
            }
        }

        [HttpPost]
        public void Post([FromForm] User user)
        {
            Config config = new Config("root", "test", "localhost", "sampledb");
            using (var connection = new ConnectionWrapper(config))
            {
                user.FirstName = user.FirstName ?? GenerateString(10);
                user.LastName = user.LastName ?? GenerateString(10);
                connection.AddUser(user.FirstName, user.LastName);
            }
        }

        private static string str = "ABCDEFGHIJKLKMOPQRSTUVWXYZ";
        private static Random rand = new Random();
        private string GenerateString(int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(str[rand.Next(0, str.Length)]);
            }
            return sb.ToString();
        }
    }
}
