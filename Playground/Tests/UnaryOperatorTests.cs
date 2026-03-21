using MiniORM.Core;

namespace Playground;

public static class UnaryOperatorTests
{
    public static void Run(OrmContext context)
    {
        var greaterThanId = 42;
        var equalsId = 44;

        var idGreaterThan = context.Where<User>(x => x.Id > greaterThanId);
        var idEquals = context.Where<User>(x => x.Id == equalsId);
        var userWithNoLastName = context.Where<User>(x => x.LastName == null);
        var usersWithLastName = context.Where<User>(x => x.LastName != null);
        var activeUsers = context.Where<User>(x => x.IsActive);
        var usersIdGreaterThanAndIsActive = context.Where<User>(x => x.Id > greaterThanId && x.IsActive);

        Console.WriteLine($"===== Users With Id Greater Than {greaterThanId} =====");
        foreach (var user in idGreaterThan)
        {
            Console.WriteLine(user);
        }

        Console.WriteLine($"===== Users With Id Equals {equalsId} =====");
        foreach (var user in idEquals)
        {
            Console.WriteLine(user);
        }

        Console.WriteLine("===== Users With Null Last Name =====");
        foreach (var user in userWithNoLastName)
        {
            Console.WriteLine(user);
        }

        Console.WriteLine($"===== Users With Last Name =====");
        foreach (var user in usersWithLastName)
        {
            Console.WriteLine(user);
        }

        Console.WriteLine($"===== Active Users =====");
        foreach (var user in activeUsers)
        {
            Console.WriteLine(user);
        }

        Console.WriteLine($"===== Active Users With Id Greater Than {greaterThanId} =====");
        foreach (var user in usersIdGreaterThanAndIsActive)
        {
            Console.WriteLine(user);
        }
    }
}