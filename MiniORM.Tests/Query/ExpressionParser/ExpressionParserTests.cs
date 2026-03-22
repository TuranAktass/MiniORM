using System.Linq.Expressions;
using MiniORM.Query.Context;
using MiniORM.Query.ExpressionParser;
using MiniORM.Tests.TestModels;

namespace MiniORM.Tests;

public class ExpressionParserTests
{
    [Fact]
    public void Parse_GreaterThanExpression_ReturnsCorrectSqlAndParameters()
    {
        var parameterContext = new QueryContext();
        var parser = new SqlExpressionParser(parameterContext);

        Expression<Func<User, bool>> expression = x => x.Id > 42;

        var result = parser.Parse(expression.Body);

        Assert.Equal("(Id > @p0)", result.Sql);
        Assert.Single(result.Parameters);
        Assert.Equal(42, result.Parameters["p0"]);
    }

    [Fact]
    public void Parse_NullComparison_ReturnsIsNull()
    {
        var parameterContext = new QueryContext();
        var parser = new SqlExpressionParser(parameterContext);

        Expression<Func<User, bool>> expression = x => x.LastName == null;

        var result = parser.Parse(expression.Body);

        Assert.Equal("LastName IS NULL", result.Sql);
        Assert.Empty(result.Parameters);
    }

    [Fact]
    public void Parse_BooleanMemberExpression_NormalizesToEqualsTrue()
    {
        var parameterContext = new QueryContext();
        var parser = new SqlExpressionParser(parameterContext);

        Expression<Func<User, bool>> expression = x => x.IsActive;

        var result = parser.Parse(expression.Body);

        Assert.Equal("(IsActive = @p0)", result.Sql);
        Assert.Single(result.Parameters);
        Assert.Equal(true, result.Parameters["p0"]);
    }

    [Fact]
    public void Parse_NotBooleanMemberExpression_ReturnsEqualsFalse()
    {
        var parameterContext = new QueryContext();
        var parser = new SqlExpressionParser(parameterContext);

        Expression<Func<User, bool>> expression = x => !x.IsActive;

        var result = parser.Parse(expression.Body);

        Assert.Equal("(IsActive = @p0)", result.Sql);
        Assert.Single(result.Parameters);
        Assert.Equal(false, result.Parameters["p0"]);
    }

    [Fact]
    public void Parse_MultipleExpressions_UsesIncrementingParameterIndexes()
    {
        var parameterContext = new QueryContext();
        var parser = new SqlExpressionParser(parameterContext);

        Expression<Func<User, bool>> expr1 = x => x.Id > 10;
        Expression<Func<User, bool>> expr2 = x => x.FirstName == "Ali";

        var result1 = parser.Parse(expr1.Body);
        var result2 = parser.Parse(expr2.Body);

        Assert.Equal("(Id > @p0)", result1.Sql);
        Assert.Equal("(FirstName = @p1)", result2.Sql);

        Assert.Equal(10, parameterContext.Parameters["p0"]);
        Assert.Equal("Ali", parameterContext.Parameters["p1"]);
    }
}