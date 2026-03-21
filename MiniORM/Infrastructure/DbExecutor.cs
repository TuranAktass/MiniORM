using System.Data.Common;
using Microsoft.Data.Sqlite;
using MiniORM.Logging;
using MiniORM.Materialization;

namespace MiniORM.Infrastructure;

public class DbExecutor
{
    private readonly string _connectionString;
    private readonly IOrmLogger _logger;

    public DbExecutor(string connectionString)
    {
        _connectionString = connectionString;
        _logger = new ConsoleOrmLogger();
    }

    public List<T> Query<T>(string sql, object? parameters = null) where T : new()
    {
        var start = DateTime.UtcNow;

        try
        {
            _logger.LogCommand(sql, parameters);

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

            var elapsed = (long)(DateTime.UtcNow - start).TotalMilliseconds;
            _logger.LogCommandCompleted(sql, elapsed);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(sql, ex);
            throw;
        }
    }

    public TResult? ExecuteScalar<TResult>(string sql, object? parameters = null)
    {
        var start = DateTime.UtcNow;

        try
        {
            _logger.LogCommand(sql, parameters);

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            AddParameters(command, parameters);

            var result = command.ExecuteScalar();

            if (result is null || result == DBNull.Value)
                return default;

            var elapsed = (long)(DateTime.UtcNow - start).TotalMilliseconds;
            _logger.LogCommandCompleted(sql, elapsed);

            return (TResult)Convert.ChangeType(result, typeof(TResult));
        }
        catch (Exception ex)
        {
            _logger.LogError(sql, ex);
            throw;
        }
    }

    public int Execute(string sql, object? parameters = null)
    {
        var start = DateTime.UtcNow;

        try
        {
            _logger.LogCommand(sql, parameters);

            using var connection = CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            AddParameters(command, parameters);

            var elapsed = (long)(DateTime.UtcNow - start).TotalMilliseconds;
            _logger.LogCommandCompleted(sql, elapsed);

            return command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _logger.LogError(sql, ex);
            throw;
        }
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