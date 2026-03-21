using MiniORM.Core;

namespace Playground.Tests;

public static class OrderByTests
{
    public static void Run(OrmContext context)
    {
        var usersAsc = context.OrderBy<User>(x => x.Id);
        var usersDesc = context.OrderByDescending<User>(x => x.Id);

        Console.WriteLine("===== USERS ASCENDING =====");
        foreach (var user in usersAsc)
        {
            Console.WriteLine(user.ToString());
        }

        Console.WriteLine("===== USERS DESCENDING =====");
        foreach (var user in usersDesc)
        {
            Console.WriteLine(user.ToString());
        }
    }
}