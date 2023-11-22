using Spark.Entities;
using Spark.UseCases;
using Spark.UseCases.SendDataToDevice;

namespace Spark.InterfaceAdapters.Consumers;

public class DeviceDataChangedConsumer<TDeviceData> where TDeviceData : IDeviceData
{
    private readonly SendDataToDevice<TDeviceData> _sendDataToDevice;

    public DeviceDataChangedConsumer(SendDataToDevice<TDeviceData> sendDataToDevice)
    {
        _sendDataToDevice = sendDataToDevice;
    }

    public async Task ConsumeAsync(DeviceDataChanged<TDeviceData> message, CancellationToken cancellationToken)
    {
        await _sendDataToDevice.ExecuteAsync(message.DeviceData);
    }
}
