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
            SelectClause = "*",
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
}