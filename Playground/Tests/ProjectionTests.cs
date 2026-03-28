using MiniORM.Core;

namespace Playground.Tests;

public static class ProjectionTests
{
    public static void Run(OrmContext context)
    {
        var result = context.Queryable<User>().Select(x =>
            new User()
            {
                FirstName = "Ali"
            }).ToList();
        Console.WriteLine("====== RESULTS ======");
        foreach (var item in result)
        {
            Console.WriteLine(item);                    
        }

        var userPhoneNumbers = context.Queryable<User>().Select(x => new { x.Id, x.FirstName, x.LastName, x.Phone}).ToList();
        Console.WriteLine("====== RESULTS WITH PHONE NUMBER ======");
        foreach (var item in userPhoneNumbers)
        {
            Console.WriteLine(item);
        }
    }
}