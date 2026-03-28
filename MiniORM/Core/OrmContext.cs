using System.Linq.Expressions;
using MiniORM.Helpers;
using MiniORM.Infrastructure;
using MiniORM.Logging;
using MiniORM.Query;

namespace MiniORM.Core;

public class OrmContext
{
    private readonly string _connectionString;
    private readonly IOrmLogger _logger;
    private readonly Func<string, IOrmLogger, DbExecutor> _executorFactory;

    public OrmContext(
        string connectionString,
        IOrmLogger? logger = null,
        Func<string, IOrmLogger, DbExecutor>? executorFactory = null)
    {
        _connectionString = connectionString;
        _logger = logger ?? new NullORMLogger();
        _executorFactory = executorFactory ?? ((cs, log) => new DbExecutor(cs, log));
    }

    private DbExecutor CreateExecutor()
    {
        return _executorFactory(_connectionString, _logger);
    }

    public QuerySet<T> Queryable<T>() where T : new()
    {
        return new QuerySet<T>(
            _connectionString,
            (cs, logger) => new DbExecutor(cs, logger),
            _logger
        );
    }

    public List<T> Query<T>(string sql, object? parameters = null) where T : new()
    {
        var executor = CreateExecutor();
        return executor.Query<T>(sql, parameters);
    }


    public int Execute(string sql, object? parameters = null)
    {
        var executor = CreateExecutor();
        return executor.Execute(sql, parameters);
    }

    public int Insert<T>(T entity)
    {
        var executor = CreateExecutor();
        var sql = MiniORM.Query.Insert.BuildInsertSql<T>();

        return executor.Execute(sql, entity);
    }

    public int Update<T>(T entity)
    {
        var executor = CreateExecutor();
        var sql = MiniORM.Query.Update.BuildUpdateSql<T>();

        return executor.Execute(sql, entity);
    }

    public int Delete<T>(T entity)
    {
        var executor = CreateExecutor();
        var sql = MiniORM.Query.Delete.BuildDeleteSql<T>();

        return executor.Execute(sql, entity);
    }

    public T? GetById<T>(int id) where T : new()
    {
        var executor = CreateExecutor();

        var entityType = typeof(T);
        var keyProperty = EntityMetaDataHelper.GetPrimaryKeyProperty(entityType);

        var sql = MiniORM.Query.GetById.BuildGetByIdSql<T>();

        var parameters = new Dictionary<string, object>
        {
            { keyProperty.Name, id }
        };

        return executor.Query<T>(sql, parameters).FirstOrDefault();
    }
}