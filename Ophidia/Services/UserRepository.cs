using Dapper;
using Microsoft.Data.Sqlite;
using Ophidia.Models;
using Ophidia.Models.Dto;

namespace Ophidia.Services
{
    public class UserRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructor that reads the connection string from appsettings.json.
        /// If it contains a relative path, it is resolved based on the ContentRootPath.
        /// Works across Windows, Linux, macOS(test?), and containers(test?).
        /// </summary>
        public UserRepository(IConfiguration configuration, IWebHostEnvironment env)
        {
            string rawConnection = configuration.GetConnectionString("Default") ?? "";

            const string dataSourcePrefix = "Data Source=";
            if (!rawConnection.StartsWith(dataSourcePrefix, StringComparison.OrdinalIgnoreCase))
            {
                string pError = "Invalid SQLite connection string format";
                throw new InvalidOperationException(pError);
            }

            string dbRelativePath = rawConnection.Substring(dataSourcePrefix.Length);
            // Example C:\Users\USER\source\repos\Ophidia\Ophidia\AppData/dev.sqlite
            string dbPath = Path.Combine(env.ContentRootPath, dbRelativePath);
            string dbFullPath = Path.GetFullPath(dbPath);

            string directory = Path.GetDirectoryName(dbFullPath) ?? "";
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _connectionString = $"Data Source={dbFullPath}";

            EnsureDatabase();
        }

        /// <summary>
        /// Fallback constructor for development use. Hardcoded to use AppData/mydb.sqlite relative to ContentRootPath.
        /// </summary>
        public UserRepository(IWebHostEnvironment env)
        {
            string dbPath = Path.Combine(env.ContentRootPath, "AppData", "mydb.sqlite");

            string directory = Path.GetDirectoryName(dbPath) ?? "";
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _connectionString = $"Data Source={dbPath}";

            EnsureDatabase();
        }

        /// <summary>
        /// Ensures the Users table exists in the database. Seeds the admin user if no records are found.
        /// </summary>
        private void EnsureDatabase()
        {
            SqliteConnection connection = new SqliteConnection(_connectionString);
            using (connection)
            {            
                connection.Open();
    
                string tableCommand = @"
    CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL
    );
    ";
                connection.Execute(tableCommand);
    
                string userCountQuery = @"SELECT COUNT(*) FROM Users";
                int count = connection.ExecuteScalar<int>(userCountQuery);
                if (count == 0)
                {
                    string insertUserCmd = @"INSERT INTO Users (Username) VALUES (@username)";
                    UserInsertParameters parameters = new UserInsertParameters()
                    {
                        Username = "admin"
                    };
                    connection.Execute(insertUserCmd, parameters);
                }
            }
        }

        /// <summary>
        /// Returns all users in the Users table.
        /// </summary>
        public IEnumerable<User> GetAllUsers()
        {
            const string sql = @"SELECT * FROM Users";
            IEnumerable<User> users;
            SqliteConnection connection = new SqliteConnection(_connectionString);
            using (connection) 
            {
                users = connection.Query<User>(sql);
            }
            return users;
        }

        /// <summary>
        /// Adds a new user to the Users table.
        /// </summary>
        public void AddUser(string username)
        {
            string insertCmd = @"INSERT INTO Users (Username) VALUES (@username)";
            UserInsertParameters parameters = new UserInsertParameters()
            {
                Username = username
            };
            SqliteConnection connection = new SqliteConnection(_connectionString);
            using (connection)
            {
                connection.Execute(insertCmd, parameters);
            }
        }
    }
}
