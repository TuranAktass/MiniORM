using MiniORM.Core;

namespace Playground.Tests;

public static class ProjectionTests
{
    public static void Run(OrmContext context)
    {
        var result = context.Queryable<User>().Select(x =>
            new
            {
                x.Id, x.FirstName
            }).ToList();
        Console.WriteLine("====== RESULTS ======");
        foreach (var item in result)
        {
            Console.WriteLine(item);
        }
    }
}