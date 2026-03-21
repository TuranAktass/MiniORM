namespace MiniORM.Query.Model;

public class QueryModel
{
    public string TableName { get; set; } = string.Empty;

    public string SelectClause { get; set; } = "*";
    public List<string> WhereClauses { get; set; } = [];
    public List<string> OrderByClauses { get; set; } = [];
    public Dictionary<string, object?> Parameters = new();
    public int? Limit { get; set; }
    public int? Offset { get; set; }


    public QueryModel Clone()
    {
        return new QueryModel()
        {
            TableName = TableName,
            SelectClause = SelectClause,
            WhereClauses = WhereClauses,
            OrderByClauses = OrderByClauses,
            Parameters = Parameters,
            Limit = Limit,
            Offset = Offset
        };
    }
}