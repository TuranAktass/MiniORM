using MiniORM.Query.ExpressionParser;
using MiniORM.Query.Model;

namespace MiniORM.Query.SqlBuilder;

public static class SelectQueryBuilder
{
    public static SqlResult Build(QueryModel model)
    {
        var selectClause = model.SelectClause.Count == 0 ? "*" : string.Join(", ", model.SelectClause);
        var sql = $"SELECT {selectClause} FROM {model.TableName}";

        if (model.WhereClauses.Any())
        {
            sql += " WHERE " + string.Join(" AND ", model.WhereClauses);
        }

        if (model.OrderByClauses.Any())
        {
            sql += " ORDER BY " + string.Join(", ", model.OrderByClauses);
        }

        if (model.Limit.HasValue)
        {
            sql += $" LIMIT {model.Limit.Value}";
        }

        if (model.Offset.HasValue)
        {
            sql += $" OFFSET {model.Offset.Value}";
        }

        return new SqlResult
        {
            Sql = sql,
            Parameters = model.Parameters
        };
    }
}