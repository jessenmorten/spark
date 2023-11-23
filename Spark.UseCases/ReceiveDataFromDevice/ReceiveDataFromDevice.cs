using Spark.Entities;

namespace Spark.UseCases.ReceiveDataFromDevice;

public class ReceiveDataFromDevice<TDevice, TDeviceData> : IUseCase<TDeviceData, TDeviceData> where TDevice : TDeviceData where TDeviceData : IDeviceData
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

    public async Task<TDeviceData> ExecuteAsync(TDeviceData request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));
        await _repository.UpdateAsync(request, cancellationToken);
        var message = new DeviceDataReceived(request.Id);
        await _messageBroker.PublishAsync(message, cancellationToken);
        return request;
    }
}
