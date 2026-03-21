using MiniORM.Core;

namespace Playground.Tests;

public static class QuerySetTest
{
    public static void Run(OrmContext context)
    {
        var allUsersButLimit1 = context.Queryable<User>().OrderBy(x => x.Id).Take(1).ToList();
        var allUsersButLimi2Offset2 = context.Queryable<User>().Skip(2).Take(1).ToList();
        var allUsers = context.Queryable<User>().ToList();
        var usersWithIdGreaterThan42 = context.Queryable<User>().Where(x => x.Id > 42).ToList();
        var userOrderedByName = context.Queryable<User>().OrderBy(x => x.FirstName).ToList();
        var activeUsersOrderedByName = context.Queryable<User>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.FirstName)
            .ToList();
        var activeAlis = context.Queryable<User>()
            .Where(x => x.IsActive)
            .Where(x => x.FirstName == "Ali")
            .ToList();

        var inactiveUsers = context.Queryable<User>()
            .Where(x => !x.IsActive)
            .ToList();

        var usersWithNullLastName = context.Queryable<User>()
            .Where(x => x.LastName == null)
            .ToList();

        var usersWithNonNullLastName = context.Queryable<User>()
            .Where(x => x.LastName != null)
            .ToList();

        var usersOrderedByIdDesc = context.Queryable<User>()
            .OrderByDescending(x => x.Id)
            .ToList();

        var user1 = context.Queryable<User>().FirstOrDefault();

        var user2 = context.Queryable<User>()
            .Where(x => x.IsActive)
            .FirstOrDefault();

        var user3 = context.Queryable<User>()
            .OrderBy(x => x.Id)
            .FirstOrDefault();

        var user4 = context.Queryable<User>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id)
            .FirstOrDefault();
    }
}