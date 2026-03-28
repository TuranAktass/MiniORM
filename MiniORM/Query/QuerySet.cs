using System.Linq.Expressions;
using MiniORM.Helpers;
using MiniORM.Infrastructure;
using MiniORM.Logging;
using MiniORM.Query.Context;
using MiniORM.Query.ExpressionParser;
using MiniORM.Query.Model;
using MiniORM.Query.SqlBuilder;

namespace MiniORM.Query;

public class QuerySet<T> where T : new()
{
    private readonly string _connectionString;
    private readonly Func<string, IOrmLogger, DbExecutor> _executorFactory;
    private readonly QueryModel _queryModel;
    private readonly QueryContext _parameterContext;
    private readonly IOrmLogger _logger;

    public QuerySet(string connectionString, Func<string, IOrmLogger, DbExecutor> executorFactory,
        IOrmLogger logger)
    {
        _connectionString = connectionString;
        _executorFactory = executorFactory;
        _logger = logger;

        _parameterContext = new QueryContext();

        _queryModel = new QueryModel()
        {
            TableName = EntityMetaDataHelper.GetTableName(typeof(T))
        };
    }

    public QuerySet<T> Select(Expression<Func<T, object>> expression)
    {
        var parsed = SelectExpressionParser.Parse(expression);
        _queryModel.BuildSelectClause(parsed);

        return this;
    }

    public QuerySet<T> Where(Expression<Func<T, bool>> expression)
    {
        var parser = new SqlExpressionParser(_parameterContext);
        var result = parser.Parse(expression.Body);

        _queryModel.WhereClauses.Add(result.Sql);
        _queryModel.Parameters = _parameterContext.Parameters;

        return this;
    }

    public QuerySet<T> OrderBy(Expression<Func<T, object>> expression)
    {
        var orderByClause = OrderByExpressionParser.Parse(expression);
        _queryModel.OrderByClauses.Add($"{orderByClause} ASC");

        return this;
    }

    public QuerySet<T> ThenBy(Expression<Func<T, object>> expression)
    {
        var orderByClause = OrderByExpressionParser.Parse(expression);
        _queryModel.OrderByClauses.Add($"{orderByClause}");

        return this;
    }

    public QuerySet<T> OrderByDescending(Expression<Func<T, object>> expression)
    {
        var orderByClause = OrderByExpressionParser.Parse(expression);
        _queryModel.OrderByClauses.Add($"{orderByClause} DESC");

        return this;
    }

    public QuerySet<T> Skip(int count)
    {
        _queryModel.Offset = count;
        return this;
    }

    public QuerySet<T> Take(int count)
    {
        _queryModel.Limit = count;
        return this;
    }

    public T? FirstOrDefault()
    {
        var queryModel = _queryModel.Clone();
        queryModel.Limit = 1;

        var sqlResult = SelectQueryBuilder.Build(queryModel);
        var executor = CreateExecutor();

        return executor.Query<T>(sqlResult.Sql, sqlResult.Parameters).FirstOrDefault();
    }

    public List<T> ToList()
    {
        var sqlResult = SelectQueryBuilder.Build(_queryModel);

        var executor = CreateExecutor();
        return executor.Query<T>(sqlResult.Sql, sqlResult.Parameters);
    }

    public bool Any()
    {
        var queryModel = _queryModel.Clone();
        queryModel.Limit = 1;
        var sqlResult = SelectQueryBuilder.Build(queryModel);
        var executor = CreateExecutor();

        var result = executor.Query<T>(sqlResult.Sql, sqlResult.Parameters);
        return result.Count != 0;
    }

    public int Count()
    {
        var queryModel = _queryModel.Clone();
        queryModel.SelectClause = ["COUNT(*)"];

        var sqlResult = SelectQueryBuilder.Build(queryModel);
        var executor = CreateExecutor();
        var count = executor.ExecuteScalar<int>(sqlResult.Sql, sqlResult.Parameters);

        return count;
    }

    private DbExecutor CreateExecutor()
    {
        return _executorFactory(_connectionString, _logger);
    }
}