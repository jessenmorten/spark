using Spark.Entities;
using Spark.UseCases;
using Spark.InterfaceAdapters.Consumers;

namespace Spark.Hub;

public class InMemoryMessageBroker<TDeviceData> : IMessageBroker where TDeviceData : IDeviceData
{
    private readonly IConsumer<DeviceDataReceived>[] _deviceDataReceivedConsumers;

    public InMemoryMessageBroker(
        IConsumer<DeviceDataReceived>[] deviceDataReceivedConsumers)
    {
        _deviceDataReceivedConsumers = deviceDataReceivedConsumers;
    }

    public async Task PublishAsync(DeviceDataReceived message, CancellationToken cancellationToken)
    {
        var tasks = _deviceDataReceivedConsumers.Select(c => c.ConsumeAsync(message, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
