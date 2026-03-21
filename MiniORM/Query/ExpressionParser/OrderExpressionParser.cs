using System.Linq.Expressions;
using System.Reflection;
using MiniORM.Helpers;

namespace MiniORM.Query.ExpressionParser;

public static class OrderByExpressionParser
{
    public static string Parse<T>(Expression<Func<T, object>> expression)
    {
        if (expression.Body is MemberExpression member &&
            member is { Expression: ParameterExpression, Member: PropertyInfo property })
        {
            return EntityMetaDataHelper.GetColumnName(property);
        }

        if (expression.Body is UnaryExpression unary &&
            unary.NodeType == ExpressionType.Convert &&
            unary.Operand is MemberExpression m &&
            m is { Expression: ParameterExpression, Member: PropertyInfo property2 })
        {
            return EntityMetaDataHelper.GetColumnName(property2);
        }

        throw new NotSupportedException(
            $"OrderBy expression '{expression.Body.NodeType}' is not supported.");
    }
}