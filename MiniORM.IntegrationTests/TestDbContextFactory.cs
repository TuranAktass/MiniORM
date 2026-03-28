using Microsoft.Data.Sqlite;
using MiniORM.Core;
using MiniORM.IntegrationTests.Models;
using MiniORM.Logging;

namespace MiniORM.IntegrationTests;

public abstract class TestDbContextFactory
{
    public static OrmContext Create()
    {
        SQLitePCL.Batteries.Init();
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();


        var context = new OrmContext(
            connection.ConnectionString,
            new NullORMLogger()
        );

        CreateSchema(connection);
        SeedData(context);

        return context;
    }

    private static void CreateSchema(SqliteConnection connection)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE Users (
                Id INTEGER PRIMARY KEY,
                FirstName TEXT,
                LastName TEXT,
                Email TEXT,
                IsActive INTEGER
            );
        ";
        cmd.ExecuteNonQuery();
    }

    private static void SeedData(OrmContext context)
    {
        context.Insert(new User { Id = 1, FirstName = "Ali", IsActive = true });
        context.Insert(new User { Id = 2, FirstName = "Veli", IsActive = false });
        context.Insert(new User { Id = 3, FirstName = "Ayşe", IsActive = true });
    }
}