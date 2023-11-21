namespace Spark.UseCases;

public interface IMessageBroker
{
    Task PublishAsync(DeviceDataReceived message, CancellationToken cancellationToken);
}
