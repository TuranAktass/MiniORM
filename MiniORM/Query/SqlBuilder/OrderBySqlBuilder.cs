using System.Linq.Expressions;
using System.Reflection;
using MiniORM.Helpers;
using MiniORM.Query.Constants;
using MiniORM.Query.ExpressionParser;

namespace MiniORM.Query.SqlBuilder;

public static class OrderBySqlBuilder
{
    public static SqlResult Build<T>(Expression<Func<T, object>> expression,
        OrderByType orderByType = OrderByType.ASC)
    {
        var tableName = EntityMetaDataHelper.GetTableName(typeof(T));
        var orderByColumnName = OrderByExpressionParser.Parse(expression);
        var type = orderByType == OrderByType.ASC ? "ASC" : "DESC";

        var sql = $"SELECT * FROM {tableName} ORDER BY {orderByColumnName} {type}";

        return new SqlResult()
        {
            Sql = sql
        };
    }
}