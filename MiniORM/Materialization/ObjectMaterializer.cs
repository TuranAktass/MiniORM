using System.Data.Common;
using System.Reflection;

namespace MiniORM.Materialization;

public static class ObjectMaterializer
{
    public static T Materialize<T>(DbDataReader reader) where T : new()
    {
        var entity = new T();

        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToList();

        for (int i = 0; i < reader.FieldCount; i++)
        {
            var columnName = reader.GetName(i);

            var property = properties.FirstOrDefault(p =>
                string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));

            if (property is null)
                continue;

            if (reader.IsDBNull(i))
                continue;

            var value = reader.GetValue(i);
            var convertedValue = ConvertToPropertyType(value, property.PropertyType);

            property.SetValue(entity, convertedValue);
        }

        return entity;
    }

    private static object? ConvertToPropertyType(object value, Type propertyType)
    {
        var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (targetType.IsEnum)
        {
            return Enum.ToObject(targetType, value);
        }

        if (targetType == typeof(Guid))
        {
            return value is Guid guid ? guid : Guid.Parse(value.ToString()!);
        }

        if (targetType == typeof(string))
        {
            return value.ToString();
        }

        return Convert.ChangeType(value, targetType);
    }
}