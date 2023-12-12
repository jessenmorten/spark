namespace Spark.UseCases;

public static class LoggerExtensions
{
    public static void Debug(this ILogger logger, string template, params object[] args)
    {
        logger.Log(LogLevel.Debug, template, null, args);
    }

    public static void Debug(this ILogger logger, string template, Exception? exception = null, params object[] args)
    {
        logger.Log(LogLevel.Debug, template, exception, args);
    }

    public static void Info(this ILogger logger, string template, params object[] args)
    {
        logger.Log(LogLevel.Info, template, null, args);
    }

    public static void Info(this ILogger logger, string template, Exception? exception = null, params object[] args)
    {
        logger.Log(LogLevel.Info, template, exception, args);
    }

    public static void Warning(this ILogger logger, string template, params object[] args)
    {
        logger.Log(LogLevel.Warning, template, null, args);
    }

    public static void Warning(this ILogger logger, string template, Exception? exception = null, params object[] args)
    {
        logger.Log(LogLevel.Warning, template, exception, args);
    }

    public static void Error(this ILogger logger, string template, params object[] args)
    {
        logger.Log(LogLevel.Error, template, null, args);
    }

    public static void Error(this ILogger logger, string template, Exception? exception = null, params object[] args)
    {
        logger.Log(LogLevel.Error, template, exception, args);
    }

    public static void Fatal(this ILogger logger, string template, params object[] args)
    {
        logger.Log(LogLevel.Fatal, template, null, args);
    }

    public static void Fatal(this ILogger logger, string template, Exception? exception = null, params object[] args)
    {
        logger.Log(LogLevel.Fatal, template, exception, args);
    }
}
