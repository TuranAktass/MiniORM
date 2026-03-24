using MiniORM.Core;

namespace Playground.Tests;

public static class ThenByTest
{
    public static void Run(OrmContext context)
    {
        var result = context.Queryable<User>().OrderBy(x => x.FirstName).ThenBy(x => x.Id)
            .ToList();

        foreach (var item in result)
        {
            Console.WriteLine(item.ToString());
        }
    }
}