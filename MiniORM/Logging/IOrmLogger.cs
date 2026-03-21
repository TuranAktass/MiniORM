namespace MiniORM.Logging;

public interface IOrmLogger
{
    void LogCommand(string sql, object? parameters);
    void LogCommandCompleted(string sql, long elapsedMs);
    void LogError(string sql, Exception exception);
}