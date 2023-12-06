namespace Spark.InterfaceAdapters.Consumers;

public interface IConsumer<T>
{
    Task ConsumeAsync(T message, CancellationToken cancellationToken);
}
