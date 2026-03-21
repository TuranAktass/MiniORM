using MiniORM.Core;

namespace Playground.Tests;

public static class MethodCallExpressionTests
{
    public static void Run(OrmContext context)
    {
        var emailEndsWithExampleCom = context.Where<User>(x => x.Email.EndsWith("com"));
        var nameStartsWithA = context.Where<User>(x => x.FirstName.StartsWith("A"));
        var nameEndsWithA = context.Where<User>(x => x.FirstName.EndsWith("a"));
        var nameContainsE = context.Where<User>(x => x.FirstName.Contains("E"));


        Console.WriteLine("===== Email Ends With example.com =====");
        foreach (var user in emailEndsWithExampleCom)
        {
            Console.WriteLine(user.ToString());
        }

        Console.WriteLine("===== Name Starts With A =====");
        foreach (var user in nameStartsWithA)
        {
            Console.WriteLine(user.ToString());
        }

        Console.WriteLine("===== Name Ends With A =====");
        foreach (var user in nameEndsWithA)
        {
            Console.WriteLine(user.ToString());
        }

        Console.WriteLine("===== Name Contains E =====");
        foreach (var user in nameContainsE)
        {
            Console.WriteLine(user.ToString());
        }
    }
}