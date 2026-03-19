using System.Linq.Expressions;
using MiniORM.Core;
using Microsoft.Data.Sqlite;
using MiniORM.Infrastructure;
using MiniORM.Query.ExpressionParser;
using Playground;


SQLitePCL.Batteries.Init();


var connectionString = "Data Source=miniorm.db";

InitializeDatabase(connectionString);

var context = new OrmContext(connectionString, cs => new DbExecutor(cs));

Expression<Func<User, bool>> expr1 = x => x.Id > 2;
Expression<Func<User, bool>> expr2 = x => x.FirstName == "Ali";
Expression<Func<User, bool>> expr3 = x => x.Id > 0 && x.FirstName == "Ali";

var expr1Res = context.Query<User>($"SELECT * FROM Users WHERE {ExpressionParser.Parse(expr1.Body)}");
var expr2Res = context.Query<User>($"SELECT * FROM Users WHERE {ExpressionParser.Parse(expr2.Body)}");
var expr3Res = context.Query<User>($"SELECT * FROM Users WHERE {ExpressionParser.Parse(expr3.Body)}");

Console.WriteLine($"EXPRESSION 1 ::: ");
foreach (var user in expr1Res)
{
    Console.WriteLine($"{user.Id} - {user.FirstName}  {user.LastName}");
}

Console.WriteLine($"EXPRESSION 2 :::");
foreach (var user in expr2Res)
{
    Console.WriteLine($"{user.Id} - {user.FirstName}  {user.LastName}");
}

Console.WriteLine($"EXPRESSION 3 :::");
foreach (var user in expr3Res)
{
    Console.WriteLine($"{user.Id} - {user.FirstName}  {user.LastName}");
}

// var newUser = new User
// {
//     FirstName = "New Adam",
//     LastName = "Çelik",
//     Email = "veli@example.com"
// };
//
// var insertedRowCount = context.Insert(newUser);
// Console.WriteLine($"Inserted rows: {insertedRowCount}");


static void InitializeDatabase(string connectionString)
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();

    using var command = connection.CreateCommand();

    command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FirstName TEXT NOT NULL,
            LastName TEXT NOT NULL,
            Email TEXT NOT NULL
        );
    ";
    command.ExecuteNonQuery();
}

/*
var users = context.Query<User>("SELECT Id, FirstName, LastName, Email FROM Users");
foreach (var user in users)
{
    Console.WriteLine($"{user.Id} - {user.FirstName} {user.LastName} - {user.Email}");
}


Console.WriteLine("=============UPDATING=============");
var userUpdate = context.Update(new User()
{
    Id = 1,
    FirstName = "Ali",
    LastName = "Yilmaz",
    Email = "aliyilar@example.com"
});
Console.WriteLine("=============NEW DATA=============");

var newUsers = context.Query<User>("SELECT Id, FirstName, LastName, Email FROM Users");
foreach (var user in newUsers)
{
    Console.WriteLine($"{user.Id} - {user.FirstName} {user.LastName} - {user.Email}");
}

Console.WriteLine("=============DELETING=============");

var userToDeleted = new User()
{
    Id = 4
};

context.Delete(userToDeleted);
Console.WriteLine("=============DATA AFTER DELETING=============");

var usersAfterDeletion = context.Query<User>("SELECT Id, FirstName, LastName, Email FROM Users");
foreach (var user in usersAfterDeletion)
{
    Console.WriteLine($"{user.Id} - {user.FirstName} {user.LastName} - {user.Email}");
}*/