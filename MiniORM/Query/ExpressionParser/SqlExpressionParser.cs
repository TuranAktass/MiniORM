using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using MiniORM.Helpers;

namespace MiniORM.Query.ExpressionParser;

public class SqlExpressionParser
{
    private int _parameterIndex;
    private Dictionary<string, object?> _parameters = new();

    public SqlResult Parse(Expression expression)
    {
        _parameterIndex = 0;
        _parameters = new Dictionary<string, object?>();

        expression = NormalizeBooleanExpression(expression);

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
            return ParseBinary(binary);
        }

        if (expression is MemberExpression member)
        {
            return ParseMember(member);
        }

        if (expression is ConstantExpression constant)
        {
            return ParseConstant(constant);
        }

        if (expression is UnaryExpression unary)
        {
            return ParseUnary(unary);
        }

        throw new NotSupportedException(
            $"Expression node type '{expression.NodeType}' is not supported.");
    }

    private string ParseBinary(BinaryExpression binary)
    {
        if (binary.NodeType == ExpressionType.Equal || binary.NodeType == ExpressionType.NotEqual)
        {
            if (IsNullConstant(binary.Left))
            {
                var column = ParseInternal(binary.Right);
                return binary.NodeType == ExpressionType.Equal
                    ? $"{column} IS NULL"
                    : $"{column} IS NOT NULL";
            }

            if (IsNullConstant(binary.Right))
            {
                var column = ParseInternal(binary.Left);
                return binary.NodeType == ExpressionType.Equal
                    ? $"{column} IS NULL"
                    : $"{column} IS NOT NULL";
            }
        }

        var leftSql = ParseInternal(binary.Left);
        var rightSql = ParseInternal(binary.Right);
        var op = ExpressionParsingHelperMethods.GetSqlOperator(binary.NodeType);

        return $"({leftSql} {op} {rightSql})";
    }

    private string ParseMember(MemberExpression expression)
    {
        if (expression is { Expression: ParameterExpression, Member: PropertyInfo property })
        {
            return EntityMetaDataHelper.GetColumnName(property);
        }

        var value = Expression.Lambda(expression).Compile().DynamicInvoke();
        return CreateParamName(value);
    }

    private string ParseUnary(UnaryExpression expression)
    {
        if (expression.NodeType == ExpressionType.Not)
        {
            if (expression.Operand is MemberExpression member && member.Type == typeof(bool))
            {
                var operand = ParseInternal(member);
                var falseParam = CreateParamName(false);
                return $"({operand} = {falseParam})";
            }

            throw new NotSupportedException(
                $"{expression.Operand.GetType().Name} is not supported.");
        }

        throw new NotSupportedException(
            $"Unary expression node type '{expression.NodeType}' is not supported.");
    }

    private string ParseConstant(ConstantExpression expression)
    {
        if (expression.Value is null)
            return "NULL";

        return CreateParamName(expression.Value);
    }

    private string CreateParamName(object? obj)
    {
        var paramName = $"p{_parameterIndex++}";
        _parameters.Add(paramName, obj);

        return "@" + paramName;
    }

    private Expression NormalizeBooleanExpression(Expression expression)
    {
        if (expression.Type == typeof(bool) && expression is MemberExpression)
        {
            return Expression.Equal(expression, Expression.Constant(true));
        }

        return expression;
    }

    private bool IsNullConstant(Expression exp)
    {
        return exp is ConstantExpression c && c.Value == null;
    }
}