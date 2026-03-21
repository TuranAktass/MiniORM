using MiniORM.Core;

namespace Playground.Tests;

public static class CountTest
{
    public static void Run(OrmContext context)
    {
        var anyUser = context.Queryable<User>().Count();
        var anyUserWithNameAli = context.Queryable<User>().Where(x => x.FirstName == "Ali").Count();
        var anyUserWithNameTuran = context.Queryable<User>().Where(x => x.FirstName == "Turan").Count();
        var activeUsers = context.Queryable<User>().Where(x => x.IsActive).Count();
        var inActive = context.Queryable<User>().Where(x => !x.IsActive).Count();


        Console.WriteLine("Count USER ::: " + anyUser);
        Console.WriteLine("Count Ali ::: " + anyUserWithNameAli);
        Console.WriteLine("Count Turan ::: " + anyUserWithNameTuran);
        Console.WriteLine("Count Active ::: " + activeUsers);
        Console.WriteLine("Count InActive ::: " + inActive);
    }
}