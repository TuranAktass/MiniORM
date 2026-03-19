using System.Data.Common;
using Microsoft.Data.Sqlite;
using MiniORM.Materialization;

namespace MiniORM.Infrastructure;

public class DbExecutor
{
    private readonly string _connectionString;

    public DbExecutor(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<T> Query<T>(string sql, object? parameters = null) where T : new()
    {
        using var connection = CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AddParameters(command, parameters);

        using var reader = command.ExecuteReader();

        var result = new List<T>();

        while (reader.Read())
        {
            var entity = ObjectMaterializer.Materialize<T>(reader);
            result.Add(entity);
        }

        return result;
    }

    public int Execute(string sql, object? parameters = null)
    {
        using var connection = CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        AddParameters(command, parameters);

        return command.ExecuteNonQuery();
    }

    private void AddParameters(DbCommand command, object? parameters)
    {
        if (parameters == null)
            return;

        if (parameters is IDictionary<string, object> dictionary)
        {
            foreach (var kv in dictionary)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@" + kv.Key;
                parameter.Value = kv.Value ?? DBNull.Value;

                command.Parameters.Add(parameter);
            }

            return;
        }

        var properties = parameters.GetType().GetProperties();

        foreach (var property in properties)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@" + property.Name;

            var value = property.GetValue(parameters);
            parameter.Value = value ?? DBNull.Value;

            command.Parameters.Add(parameter);
        }
    }

    private DbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}