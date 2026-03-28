namespace MiniORM.Query.Model;

public class QueryModel
{
    public string TableName { get; set; } = string.Empty;

    public List<string> SelectClause { get; set; } = [];
    public List<string> WhereClauses { get; set; } = [];
    public List<string> OrderByClauses { get; set; } = [];
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public int? Limit { get; set; }
    public int? Offset { get; set; }


    public void BuildSelectClause(List<ProjectionModel> projections)
    {
        SelectClause = projections
            .Select(x => $"{x.ColumnName} AS {x.TargetName}")
            .ToList();
    }

    public QueryModel Clone()
    {
        return new QueryModel()
        {
            TableName = TableName,
            SelectClause = [..SelectClause],
            WhereClauses = [..WhereClauses],
            OrderByClauses = [..OrderByClauses],
            Parameters = new Dictionary<string, object?>(Parameters),
            Limit = Limit,
            Offset = Offset
        };
    }
}