using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Security.Infrastructure.Data
{
    /// <summary>
    /// Database connector class
    /// </summary>
    public class DbConnector
    {
        private readonly IConfiguration _configuration;

        protected DbConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Create connection instance
        /// </summary>
        public IDbConnection CreateConnection()
        {
            string _connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(_connectionString);
        }
    }
}