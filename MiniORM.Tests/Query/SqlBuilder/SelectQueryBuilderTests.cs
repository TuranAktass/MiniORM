using MiniORM.Query.Model;
using MiniORM.Query.SqlBuilder;

namespace MiniORM.Tests.Query.SqlBuilder;

public class SelectQueryBuilderTests
{
    [Fact]
    public void Build_WithWhereOrderByLimitOffset_ReturnsCorrectSql()
    {
        var model = new QueryModel
        {
            TableName = "Users",
            WhereClauses = new List<string> { "(IsActive = @p0)" },
            OrderByClauses = new List<string> { "FirstName ASC" },
            Parameters = new Dictionary<string, object?> { ["p0"] = true },
            Limit = 10,
            Offset = 20
        };

        var result = SelectQueryBuilder.Build(model);

        Assert.Equal(
            "SELECT * FROM Users WHERE (IsActive = @p0) ORDER BY FirstName ASC LIMIT 10 OFFSET 20",
            result.Sql);

        Assert.Single(result.Parameters);
        Assert.Equal(true, result.Parameters["p0"]);
    }

    [Fact]
    public void Build_WithWhereOrderByThenByLimitOffset_ReturnsCorrectSql()
    {
        var model = new QueryModel
        {
            TableName = "Users",
            WhereClauses = new List<string> { "(IsActive = @p0)" },
            OrderByClauses = new List<string> { "FirstName, LastName ASC" },
            Parameters = new Dictionary<string, object?> { ["p0"] = true },
            Limit = 10,
            Offset = 20
        };

        var result = SelectQueryBuilder.Build(model);

        Assert.Equal(
            "SELECT * FROM Users WHERE (IsActive = @p0) ORDER BY FirstName, LastName ASC LIMIT 10 OFFSET 20",
            result.Sql);

        Assert.Single(result.Parameters);
        Assert.Equal(true, result.Parameters["p0"]);
    }

    [Fact]
    public void Build_ProjectionWithWhereOrderByThenByLimitOffset_ReturnsCorrectSql()
    {
        var model = new QueryModel
        {
            TableName = "Users",
            SelectClause = ["Id, FirstName, LastName"],
            WhereClauses = new List<string> { "(IsActive = @p0)" },
            OrderByClauses = new List<string> { "FirstName, LastName ASC" },
            Parameters = new Dictionary<string, object?> { ["p0"] = true },
            Limit = 10,
            Offset = 20
        };

        var result = SelectQueryBuilder.Build(model);

        Assert.Equal(
            "SELECT Id, FirstName, LastName FROM Users WHERE (IsActive = @p0) ORDER BY FirstName, LastName ASC LIMIT 10 OFFSET 20",
            result.Sql);

        Assert.DoesNotContain("SELECT *", result.Sql);
        Assert.DoesNotContain("Email", result.Sql);
        Assert.Single(result.Parameters);
        Assert.Equal(true, result.Parameters["p0"]);
    }
}