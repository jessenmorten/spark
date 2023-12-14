using Spark.Entities;
using Spark.UseCases;
using Spark.InterfaceAdapters.Consumers;

namespace Spark.Hub;

public class InMemoryMessageBroker<TDeviceData> : IMessageBroker<TDeviceData> where TDeviceData : IDeviceData
{
    private readonly IConsumer<DeviceDataReceived>[] _deviceDataReceivedConsumers;
    private readonly IConsumer<DeviceDataChanged<TDeviceData>>[] _deviceDataChangedConsumers;

    public InMemoryMessageBroker(
        IConsumer<DeviceDataReceived>[] deviceDataReceivedConsumers,
        IConsumer<DeviceDataChanged<TDeviceData>>[] deviceDataChangedConsumers)
    {
        _deviceDataReceivedConsumers = deviceDataReceivedConsumers;
        _deviceDataChangedConsumers = deviceDataChangedConsumers;
    }

    public async Task PublishAsync(DeviceDataReceived message, CancellationToken cancellationToken)
    {
        var tasks = _deviceDataReceivedConsumers.Select(c => c.ConsumeAsync(message, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task PublishAsync(DeviceDataChanged<TDeviceData> message, CancellationToken cancellationToken)
    {
        var tasks = _deviceDataChangedConsumers.Select(c => c.ConsumeAsync(message, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
