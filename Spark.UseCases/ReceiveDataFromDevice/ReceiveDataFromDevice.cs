using Spark.Entities;

namespace Spark.UseCases.ReceiveDataFromDevice;

public class ReceiveDataFromDevice<TDevice, TDeviceData> where TDevice : TDeviceData where TDeviceData : IDeviceData
{
    private readonly IRepository<TDevice, TDeviceData> _repository;
    private readonly IMessageBroker _messageBroker;

    public ReceiveDataFromDevice(
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
        var message = new DeviceDataReceived(device.Id);
        await _messageBroker.PublishAsync(message, cancellationToken);
    }
}
