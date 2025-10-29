
/*using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace BeerCraftMVC.Data
{
    /// <summary>
    /// Singleton клас за работа с базата данни на BeerCraftMVC.
    /// Използва SQLite и предоставя CRUD операции за всички модели.
    /// </summary>
    public sealed class Database
    {
        private static readonly Lazy<Database> _instance = new Lazy<Database>(() => new Database());
        private readonly string _connectionString;
        private const string DatabaseFileName = "beercraft.db";

        private Database()
        {
            // Set up SQLite database file path
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseFileName);
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        public static Database Instance => _instance.Value;

        private void InitializeDatabase()
        {
            if (!File.Exists(DatabaseFileName))
            {
                SQLiteConnection.CreateFile(DatabaseFileName);
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string tableCmd = @"
                CREATE TABLE IF NOT EXISTS User(
                Id INTEGER PRIMARY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Username TEXT NOT NULL,
                Email TEXT NOT NULL,
                CreaedAt TEXT NOT NULL,
                HashedPassword TEXT NOT NULL)";
                using (var command = new SQLiteCommand(tableCmd, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
*/