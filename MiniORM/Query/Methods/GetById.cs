using MiniORM.Helpers;

namespace MiniORM.Query;

class GetById
{
    public static string BuildGetByIdSql<T>()
    {
        var entityType = typeof(T);

        var tableName = EntityMetaDataHelper.GetTableName(entityType);
        var keyProperty = EntityMetaDataHelper.GetPrimaryKeyProperty(entityType);
        var keyColumn = EntityMetaDataHelper.GetColumnName(keyProperty);

        return $"SELECT * FROM {tableName} WHERE {keyColumn} = @{keyProperty.Name}";
    }
}