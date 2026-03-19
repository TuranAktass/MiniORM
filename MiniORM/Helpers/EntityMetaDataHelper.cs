using System.Reflection;
using MiniORM.Mapping;

namespace MiniORM.Helpers;

public static class EntityMetaDataHelper
{
    public static string GetTableName(Type entityType)
    {
        var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();
        return tableAttribute?.Name ?? entityType.Name;
    }

    public static string GetColumnName(PropertyInfo property)
    {
        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
        return columnAttribute?.Name ?? property.Name;
    }

    public static PropertyInfo GetPrimaryKeyProperty(Type entityType)
    {
        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var keyProperty = properties.FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);
        if (keyProperty != null)
            return keyProperty;

        keyProperty = properties
            .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        if (keyProperty == null)
            throw new Exception($"Theres no key property for entity : {entityType.Name}");

        return keyProperty;
    }

    public static List<PropertyInfo> GetInsertableProperties(Type entityType)
    {
        return entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p is { CanRead: true, CanWrite: true })
            .Where(p => p.GetCustomAttribute<KeyAttribute>() is null)
            .ToList();
    }
    
    public static List<PropertyInfo> GetUpdatableProperties(Type entityType)
    {
        var keyProperty = GetPrimaryKeyProperty(entityType);

        return entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .Where(p => p != keyProperty)
            .ToList();
    }
}