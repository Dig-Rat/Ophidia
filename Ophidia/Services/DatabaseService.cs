using Microsoft.Data.Sqlite;
using System.Data;

namespace Ophidia.Services
{    
    public class DatabaseService
    {
        private readonly string _connectionString;

        // Default constructor, primes the connection string.
        public DatabaseService()
        {
            // Use a real path or appsettings.json later
            _connectionString = "Data Source=AppData/mydb.sqlite";
            //_connectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "AppData", "mydb.sqlite")}";
        }

        // --
        public List<string> GetUsernames()
        {
            List<string> usernames = new();
            string queryText = @"
SELECT
Username 
FROM Users;
";
            SqliteConnection connection = new SqliteConnection(_connectionString);
            using (connection)
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = queryText;
                SqliteDataReader reader;
                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usernames.Add(reader.GetString(0));
                    }
                }
            }
            return usernames;
        }

        // --
        public void EnsureDatabaseCreated()
        {
            string createTables = @"
CREATE TABLE IF NOT EXISTS Users 
(
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Username TEXT NOT NULL
);
";

            string insertAdmin = @"
INSERT INTO Users (Username) VALUES ('admin');
";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            using (connection)
            {
                connection.Open();
                SqliteCommand tableCmd = connection.CreateCommand();
                tableCmd.CommandText = createTables;
                tableCmd.ExecuteNonQuery();
                SqliteCommand insertCmd = connection.CreateCommand();
                insertCmd.CommandText = insertAdmin;
                insertCmd.ExecuteNonQuery();
            }
        }


    }

}
