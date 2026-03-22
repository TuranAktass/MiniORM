using System.Linq.Expressions;
using MiniORM.Query.ExpressionParser;
using MiniORM.Tests.TestModels;
using Xunit;

namespace MiniORM.Tests.Query.ExpressionParser;

public class OrderExpressionParserTests
{
    [Fact]
    public void OrderByParse_StringProperty_ReturnsColumnName()
    {
        Expression<Func<User, object>> expression = x => x.FirstName;

        var result = OrderByExpressionParser.Parse(expression);

        Assert.Equal("FirstName", result);
    }

    [Fact]
    public void OrderByParse_IntPropertyWrappedInConvert_ReturnsColumnName()
    {
        Expression<Func<User, object>> expression = x => x.Id;

        var result = OrderByExpressionParser.Parse(expression);
        Assert.Equal("Id", result);
    }
}