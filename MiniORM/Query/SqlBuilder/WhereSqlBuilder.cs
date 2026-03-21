using System.Linq.Expressions;
using MiniORM.Helpers;
using MiniORM.Query.ExpressionParser;

namespace MiniORM.Query.SqlBuilder;

public static class WhereSqlBuilder
{
    public static SqlResult Build<T>(Expression<Func<T, bool>> expression)
    {
        SqlExpressionParser parser = new SqlExpressionParser();

        var tableName = EntityMetaDataHelper.GetTableName(typeof(T));
        var parsedExpression = parser.Parse(expression.Body);

        var sql = $"SELECT * FROM {tableName} WHERE {parsedExpression.Sql}";
        return new SqlResult()
        {
            Sql = sql,
            Parameters = parsedExpression.Parameters
        };
    }
}