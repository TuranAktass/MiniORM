namespace MiniORM.Query.ExpressionParser;

public class SqlResult
{
    public string Sql { get; set; }
    public Dictionary<string, object?> Parameters { get; set; }

    public string DebugLog()
    {
        return $"SQL ::: {Sql} --- Parameters ::: {ParamsToString()}";
    }

    private string ParamsToString()
    {
        var paramsToStr = "";
        foreach (var param in Parameters)
        {
            paramsToStr += param + ", ";
        }

        return paramsToStr;
    }
}