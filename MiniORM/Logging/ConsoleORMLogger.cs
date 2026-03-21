namespace MiniORM.Logging;

public class ConsoleOrmLogger : IOrmLogger
{
    public void LogCommand(string sql, object? parameters)
    {
        Console.WriteLine($"[MiniORM] Executing SQL: {sql}");

        if (parameters is not null)
        {
            Console.WriteLine($"[MiniORM] Parameters: {FormatParameters(parameters)}");
        }
    }

    public void LogCommandCompleted(string sql, long elapsedMs)
    {
        Console.WriteLine($"[MiniORM] Completed in {elapsedMs} ms");
    }

    public void LogError(string sql, Exception exception)
    {
        Console.WriteLine($"[MiniORM] ERROR executing SQL: {sql}");
        Console.WriteLine($"[MiniORM] Exception: {exception.Message}");
    }

    private string FormatParameters(object parameters)
    {
        if (parameters is Dictionary<string, object?> dict)
        {
            return string.Join(", ", dict.Select(x => $"{x.Key}={x.Value}"));
        }

        return parameters.ToString() ?? "";
    }
}