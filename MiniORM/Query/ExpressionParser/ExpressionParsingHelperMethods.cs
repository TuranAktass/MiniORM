using System.Linq.Expressions;

namespace MiniORM.Query.ExpressionParser;

public static class ExpressionParsingHelperMethods
{
    public static string GetSqlOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<=",
            ExpressionType.And => "AND",
            ExpressionType.Or => "OR",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            
            _ => throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null)
        };
    }
}