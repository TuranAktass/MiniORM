using MiniORM.Core;

namespace Playground.Tests;

public static class AnyTest
{
    public static void Run(OrmContext context)
    {
        var anyUser = context.Queryable<User>().Any();
        var anyUserWithNameAli = context.Queryable<User>().Where(x => x.FirstName == "Ali").Any();
        var anyUserWithNameTuran = context.Queryable<User>().Where(x => x.FirstName == "Turan").Any();

        Console.WriteLine("ANY USER ::: " + anyUser);
        Console.WriteLine("ANY Ali ::: " + anyUserWithNameAli);
        Console.WriteLine("ANY Turan ::: " + anyUserWithNameTuran);
    }
}