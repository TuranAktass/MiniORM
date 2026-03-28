namespace MiniORM.Logging;

public class NullORMLogger : IOrmLogger
{
    public void LogCommand(string sql, object? parameters)
    {
    }

    public void LogCommandCompleted(string sql, long elapsedMs)
    {
    }

    public void LogError(string sql, Exception exception)
    {
    }
}