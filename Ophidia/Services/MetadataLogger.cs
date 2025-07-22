using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Ophidia.Models;

namespace Ophidia.Services
{
    public class MetadataLogger
    {
        private readonly string _connectionString;

        // --
        public MetadataLogger(string connectionString)
        {
            _connectionString = connectionString;
        }

        // --
        public void Log(VisitorMetadata metadata)
        {
            string sql = @"
                INSERT INTO VisitorMetadata 
                (Path, Method, UserAgent, Referrer, DeviceType, ScreenResolution, Timestamp) 
                VALUES 
                (@Path, @Method, @UserAgent, @Referrer, @DeviceType, @ScreenResolution, @Timestamp);"
            ;
            SqliteConnection connection = new SqliteConnection(_connectionString);
            using (connection)
            {
                connection.Open();
                connection.Execute(sql, metadata);
            }
        }

        // --
        public void CreateTable()
        {
            const string sql = @"
CREATE TABLE VisitorMetadata (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Path TEXT NOT NULL,
    Method TEXT NOT NULL,
    UserAgent TEXT NOT NULL,
    Referrer TEXT,
    DeviceType TEXT,
    ScreenResolution TEXT,
    Timestamp TEXT NOT NULL
);
";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            using (connection)
            {
                connection.Open();
                connection.Execute(sql);
            }

        }
    }
}
