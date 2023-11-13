namespace Spark.UseCases;

public interface IMessageBroker
{
    Task PublishAsync(DeviceSyncEvent e, CancellationToken cancellationToken);
}
