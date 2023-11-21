using Spark.Entities;

namespace Spark.UseCases.SyncDeviceState;

public class SyncDeviceState<TDevice, TDeviceData> where TDevice : TDeviceData where TDeviceData : IDeviceData
{
    private readonly IRepository<TDevice, TDeviceData> _repository;
    private readonly IMessageBroker _messageBroker;

    public SyncDeviceState(
        IRepository<TDevice, TDeviceData> repository,
        IMessageBroker eventBus)
    {
        _repository = repository;
        _messageBroker = eventBus;
    }

    public async Task ExecuteAsync(TDeviceData device, CancellationToken cancellationToken)
    {
        _ = device ?? throw new ArgumentNullException(nameof(device));
        await _repository.UpdateAsync(device, cancellationToken);
        var syncEvent = new DeviceSyncEvent(device.Id);
        await _messageBroker.PublishAsync(syncEvent, cancellationToken);
    }
}
