using MiniORM.Helpers;

namespace MiniORM.Query;

public class Update
{
    public static string BuildUpdateSql<T>()
    {
        var entityType = typeof(T);

        var tableName = EntityMetaDataHelper.GetTableName(entityType);
        var keyProperty = EntityMetaDataHelper.GetPrimaryKeyProperty(entityType);
        var keyColumn = EntityMetaDataHelper.GetColumnName(keyProperty);

        var properties = EntityMetaDataHelper.GetUpdatableProperties(entityType);

        var setClause = string.Join(", ",
            properties.Select(p => $"{EntityMetaDataHelper.GetColumnName(p)} = @{p.Name}")
        );

        var sql =
            $"UPDATE {tableName} " +
            $"SET {setClause} " +
            $"WHERE {keyColumn} = @{keyProperty.Name}";

        return sql;
    }
    
}