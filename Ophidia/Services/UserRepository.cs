using Dapper;
using Microsoft.Data.Sqlite;
using Ophidia.Models;

namespace Ophidia.Services
{
    public class UserRepository
    {
        private readonly string _connectionString;

        // --
        public UserRepository(IConfiguration configuration, IWebHostEnvironment env)
        {
            string rawConnection = configuration.GetConnectionString("Default") ?? "";

            // If using a relative path in appsettings, resolve it from content root
            if (rawConnection.Contains("AppData"))
            {
                string fullPath = Path.Combine(env.ContentRootPath, "AppData", "mydb.sqlite");
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                _connectionString = $"Data Source={fullPath}";
            }
            else
            {
                _connectionString = rawConnection;
            }

            EnsureDatabase();
        }

        // --
        public UserRepository(IWebHostEnvironment env)
        {
            string dbPath = Path.Combine(env.ContentRootPath, "AppData", "mydb.sqlite");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            _connectionString = $"Data Source={dbPath}";

            EnsureDatabase();
        }

        // --
        private void EnsureDatabase()
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string tableCommand = @"
CREATE TABLE IF NOT EXISTS Users (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Username TEXT NOT NULL
);
";
            connection.Execute(tableCommand);

            // Seed one row if empty
            int count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Users");
            if (count == 0)
            {
                connection.Execute("INSERT INTO Users (Username) VALUES (@name)", new { name = "admin" });
            }
        }

        // --
        public IEnumerable<User> GetAllUsers()
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            return connection.Query<User>("SELECT * FROM Users");
        }

        // --
        public void AddUser(string username)
        {
            string insertCmd = @"INSERT INTO Users (Username) VALUES (@username)";
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Execute(insertCmd, new { username });
        }
    }
}
