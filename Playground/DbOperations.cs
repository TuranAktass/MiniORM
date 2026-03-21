using Microsoft.Data.Sqlite;
using MiniORM.Core;

namespace Playground;

public static class DbOperations
{
    public static void DropTables(OrmContext context)
    {
        context.Execute("DROP TABLE IF EXISTS Users");
    }

    public static void DeleteTable(OrmContext context, string tableName)
    {
        context.Execute($"DELETE FROM {tableName}");
    }

    public static void Seed(OrmContext context)
    {
        var users = new List<User>
        {
            new User
            {
                FirstName = "Ali",
                LastName = "Yılmaz",
                Email = "ali@example.com",
                IsActive = true
            },
            new User
            {
                FirstName = "Veli",
                LastName = "Kaya",
                Email = "veli@example.com",
                IsActive = true
            },
            new User
            {
                FirstName = "Ayşe",
                LastName = "Demir",
                Email = "ayse@example.com",
                IsActive = false
            },
            new User
            {
                FirstName = "Fatma",
                LastName = null, // NULL test
                Email = "fatma@example.com",
                IsActive = true
            },
            new User
            {
                FirstName = "Mehmet",
                LastName = "Çelik",
                Email = "mehmet@example.com",
                IsActive = false
            }
        };

        foreach (var user in users)
        {
            context.Insert(user);
        }
    }

    public static void InitializeDatabase(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FirstName TEXT NOT NULL,
            LastName TEXT,
            IsActive BYTE,
            Email TEXT NOT NULL
        );
    ";
        command.ExecuteNonQuery();
    }
}