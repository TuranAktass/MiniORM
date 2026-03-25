using System.Linq.Expressions;
using MiniORM.Query.ExpressionParser;
using MiniORM.Tests.TestModels;

namespace MiniORM.Tests.Query.ExpressionParser;

public class SelectExpressionParserTests
{
    [Fact]
    public void SelectProjectionParse_StringProperty_ReturnsColumnName()
    {
        Expression<Func<User, object>> expression = x => new { x.FirstName, x.Id };

        var result = SelectExpressionParser.Parse(expression);
        var list = new List<string> { "FirstName" , "Id"};
        Assert.Equal(result, list);
        
    }
}