using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using MiniORM.Helpers;

namespace MiniORM.Query.ExpressionParser;

public class ExpressionParser
{
    private int _parameterIndex;
    private Dictionary<string, object?> _parameters = new();

    public SqlResult Parse(Expression expression)
    {
        _parameterIndex = 0;
        _parameters = new Dictionary<string, object?>();

        var sql = ParseInternal(expression);

        return new SqlResult()
        {
            Sql = sql,
            Parameters = _parameters,
        };
    }

    private string ParseInternal(Expression expression)
    {
        if (expression is BinaryExpression binary)
        {
            var left = ParseInternal(binary.Left);
            var right = ParseInternal(binary.Right);
            var op = ExpressionParsingHelperMethods.GetSqlOperator(binary.NodeType);

            // Console.WriteLine( $"({left} {op} {right})");
            return $"({left} {op} {right})";
        }

        if (expression is MemberExpression member)
        {
            return ParseMember(member);
        }

        if (expression is ConstantExpression constant)
        {
            return ParseConstant(constant);
        }

        throw new Exception($"Expression type '{{expression.GetType().Name}}' is not supported.\"");
    }

    private string ParseMember(MemberExpression expression)
    {
        if (expression.Expression is not ParameterExpression parameter)
            throw new NotSupportedException("");

        return expression.Member is not PropertyInfo property
            ? throw new NotSupportedException($"{expression.Member.Name} is not property")
            : EntityMetaDataHelper.GetColumnName(property);
    }

    private string ParseConstant(ConstantExpression expression)
    {
        if (expression.Value is null)
            throw new NoNullAllowedException(
                $"{expression.Value} is [NULL], need to be fixed for supporting null values");
        return CreateParamName(expression.Value);
    }

    private string CreateParamName(object? obj)
    {
        var paramName = $"p{_parameterIndex++}";
        _parameters.Add(paramName, obj);

        return "@" + paramName;
    }
}