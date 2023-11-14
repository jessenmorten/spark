namespace Spark.UnitTests;

public static class TaskExtensions
{
    public static async Task<T> Delay<T>(this Task<T> task, int ms = 1000)
    {
        await Task.Delay(ms);
        return await task;
    }
}
