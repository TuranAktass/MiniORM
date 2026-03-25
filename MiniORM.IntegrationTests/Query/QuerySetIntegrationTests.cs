using MiniORM.IntegrationTests.Models;

namespace MiniORM.IntegrationTests.Query;

/*context.Insert(new User { Id = 1, FirstName = "Ali", IsActive = true });
context.Insert(new User { Id = 2, FirstName = "Veli", IsActive = false });
context.Insert(new User { Id = 3, FirstName = "Ayşe", IsActive = true });*/

public class QuerySetIntegrationTests
{
    [Fact]
    public void Where_IsActive_ReturnsOnlyActiveUsers()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>()
            .Where(x => x.IsActive)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.True(x.IsActive));
    }

    [Fact]
    public void OrderBy_FirstName_ReturnsSortedUsers()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>().OrderBy(x => x.FirstName).ToList();

        Assert.Equal("Ali", result.First().FirstName);
        Assert.Equal("Ayşe", result[1].FirstName);
    }

    [Fact]
    public void FirstOrDefault_ReturnsFirstMatchingUser()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>().Where(x => x.FirstName == "Ali").FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal("Ali", result.FirstName);
    }

    [Fact]
    public void Any_ReturnsTrue_WhenMatchExists()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>().Where(x => x.FirstName == "Ali").Any();

        Assert.True(result);
    }

    [Fact]
    public void Count_ReturnsCorrectUserNumber()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>().Count();

        Assert.Equal(3, result);
    }

    [Fact]
    public void MethodCallExpression_Contains_ReturnsCorrectData()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>().Where(x => x.FirstName.Contains("A")).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void MethodCallExpression_StartsWith_ReturnsCorrectData()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>().Where(x => x.FirstName.StartsWith("A")).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void MethodCallExpression_EndsWith_ReturnsCorrectData()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>().Where(x => x.FirstName.EndsWith("e")).ToList();

        Assert.Equal(1, result.Count);
    }

    [Fact]
    public void OrderByAndThenBy_ReturnsUsersInCorrectOrder()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id).ThenBy(x => x.FirstName).ToList();

        var expected = result
            .OrderBy(x => x.Id)
            .ThenBy(x => x.FirstName)
            .ToList();

        Assert.True(result.SequenceEqual(expected));
    }

    [Fact]
    public void SelectWithProjection_ReturnsDataCorrectly()
    {
        var context = TestDbContextFactory.Create();

        var result = context.Queryable<User>()
            .Where(x => x.IsActive)
            .Select(x => new { x.Id, x.FirstName })
            .ToList();

        Assert.NotEmpty(result);
    }
}