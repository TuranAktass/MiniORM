using System.Linq.Expressions;
using MiniORM.Query.ExpressionParser;
using MiniORM.Query.Model;
using MiniORM.Tests.TestModels;

namespace MiniORM.Tests.Query.ExpressionParser;

public class SelectExpressionParserTests
{
    [Fact]
    public void SelectProjectionParse_StringProperty_ReturnsColumnName()
    {
        Expression<Func<User, object>> expression = x => new { x.FirstName };

        var result = SelectExpressionParser.Parse(expression);
        var list = new List<ProjectionModel>([
            new ProjectionModel()
                { TargetName = "FirstName", ColumnName = "FirstName" }
        ]);
        Assert.Equivalent(result, list);
    }

    [Fact]
    public void SelectWithDifferentColumnAndPropertyNameProjection_ReturnsCorrectSql()
    {
        Expression<Func<User, object>> expression = x => new { x.Phone };

        var result = SelectExpressionParser.Parse(expression);
        var list = new List<ProjectionModel>([
            new ProjectionModel()
                { TargetName = "Phone", ColumnName = "PhoneNumber" }
        ]);

        Assert.Equivalent(result, list);
    }
}