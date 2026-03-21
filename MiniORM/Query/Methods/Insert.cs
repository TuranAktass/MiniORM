using MiniORM.Helpers;

namespace MiniORM.Query;

public static class Insert
{
    public static string BuildInsertSql<T>()
    {
        var entityType = typeof(T);

        var tableName = EntityMetaDataHelper.GetTableName(entityType);
        var properties = EntityMetaDataHelper.GetInsertableProperties(entityType);

        var columnNames = properties
            .Select(EntityMetaDataHelper.GetColumnName)
            .ToList();

        var parameterNames = properties
            .Select(p => "@" + p.Name)
            .ToList();

        var columnsPart = string.Join(", ", columnNames);
        var valuesPart = string.Join(", ", parameterNames);

        return $"INSERT INTO {tableName} ({columnsPart}) VALUES ({valuesPart})";
    }

}