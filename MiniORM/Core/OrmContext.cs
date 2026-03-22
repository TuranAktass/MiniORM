using System.Linq.Expressions;
using MiniORM.Helpers;
using MiniORM.Infrastructure;
using MiniORM.Query;

namespace MiniORM.Core;

public class OrmContext
{
    private readonly string _connectionString;
    private readonly Func<string, DbExecutor> _executorFactory;

    public OrmContext(string connectionString, Func<string, DbExecutor> executorFactory)
    {
        _connectionString = connectionString;
        _executorFactory = executorFactory;
    }

    public QuerySet<T> Queryable<T>() where T : new()
    {
        return new QuerySet<T>(_connectionString, _executorFactory);
    }

    public List<T> Query<T>(string sql, object? parameters = null) where T : new()
    {
        var executor = _executorFactory(_connectionString);
        return executor.Query<T>(sql, parameters);
    }


    public int Execute(string sql, object? parameters = null)
    {
        var executor = _executorFactory(_connectionString);
        return executor.Execute(sql, parameters);
    }

    public int Insert<T>(T entity)
    {
        var executor = _executorFactory(_connectionString);
        var sql = MiniORM.Query.Insert.BuildInsertSql<T>();

        return executor.Execute(sql, entity);
    }

    public int Update<T>(T entity)
    {
        var executor = _executorFactory(_connectionString);
        var sql = MiniORM.Query.Update.BuildUpdateSql<T>();

        return executor.Execute(sql, entity);
    }

    public int Delete<T>(T entity)
    {
        var executor = _executorFactory(_connectionString);
        var sql = MiniORM.Query.Delete.BuildDeleteSql<T>();

        return executor.Execute(sql, entity);
    }

    public T? GetById<T>(int id) where T : new()
    {
        var executor = _executorFactory(_connectionString);

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