namespace MiniORM.Query.Context;

public class QueryContext
{
    public int ParameterIndex { get; set; }
    public Dictionary<string, object> Parameters = new();
}