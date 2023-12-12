namespace Spark.UseCases;

public interface ILogger
{
    void Log(LogLevel level, string template, Exception? exception = null, params object[] args);
}
