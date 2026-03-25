using System.Linq.Expressions;
using System.Reflection;
using MiniORM.Helpers;

namespace MiniORM.Query.ExpressionParser;

public static class SelectExpressionParser
{
    public static List<string> Parse<T>(Expression<Func<T, object>> expression)
    {
        if (expression.Body is NewExpression newExpression)
        {
            if (newExpression.Members != null)
                return newExpression.Members.Select(m => m.Name).ToList();

            throw new NotSupportedException("NewExpression with Null [Members] is not supported.");
        }

        throw new NotSupportedException(
            $"OrderBy expression '{expression.Body.NodeType}' is not supported.");
    }
}