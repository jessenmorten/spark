using Spark.Entities;

namespace Spark.UseCases;

public interface IMessageBroker<TDeviceData> where TDeviceData : IDeviceData
{
    Task PublishAsync(DeviceDataReceived message, CancellationToken cancellationToken);
    Task PublishAsync(DeviceDataChanged<TDeviceData> message, CancellationToken cancellationToken);
}
