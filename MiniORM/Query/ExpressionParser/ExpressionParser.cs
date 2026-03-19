using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using MiniORM.Helpers;

namespace MiniORM.Query.ExpressionParser;

public static class ExpressionParser
{
    public static string Parse(Expression expression)
    {
        if (expression is BinaryExpression binary)
        {
            var columnName = Parse(binary.Left);
            var right = Parse(binary.Right);
            var op = ExpressionParsingHelperMethods.GetSqlOperator(binary.NodeType);

            return $"({columnName} {op} {right})";
        }

        if (expression is MemberExpression member)
        {
            return ParseMember(member);
        }

        if (expression is ConstantExpression constant)
        {
            return ParseConstant(constant);
        }

        else
        {
            throw new Exception($"EXPECTED BINARY EXPRESSION GOT {expression.Type}");
        }
    }


    private static string ParseMember(MemberExpression expression)
    {
        if (expression.Member is not PropertyInfo property)
        {
            throw new NotSupportedException($"{expression.Member.Name} is not property");
        }

        return EntityMetaDataHelper.GetColumnName(property);
    }

    private static string ParseConstant(ConstantExpression expression)
    {
        return expression.Value switch
        {
            string str => $"'{str}'",
            bool b => b ? "1" : "0",
            _ => expression.Value?.ToString() ?? "NULL"
        };
    }
}