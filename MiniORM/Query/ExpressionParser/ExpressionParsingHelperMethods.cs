using System.Linq.Expressions;

namespace MiniORM.Query.ExpressionParser;

public static class ExpressionParsingHelperMethods
{
    public static string GetSqlOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",

            _ => throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null)
        };
    }

    public static object? GetValue(Expression expression)
    {
        if (expression is ConstantExpression constant)
            return constant.Value;

        return Expression.Lambda(expression).Compile().DynamicInvoke();
    }

    // public static string GetColumnName(Expression expression)
    // {
    //     if (expression.NodeType == ExpressionType.Convert convert){
    //     }
    // }
}