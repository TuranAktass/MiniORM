using MiniORM.Helpers;

namespace MiniORM.Query;

public class Delete
{
    public static string BuildDeleteSql<T>()
    {
        var entityType = typeof(T);

        var tableName = EntityMetaDataHelper.GetTableName(entityType);
        var keyProperty = EntityMetaDataHelper.GetPrimaryKeyProperty(entityType);
        var keyColumn = EntityMetaDataHelper.GetColumnName(keyProperty);
        

        var sql =
            $"DELETE FROM {tableName} " +
            $"WHERE {keyColumn} = @{keyProperty.Name}";

        return sql;
    }
    
}