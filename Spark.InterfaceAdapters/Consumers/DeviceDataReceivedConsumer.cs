using Spark.Entities;
using Spark.UseCases;
using Spark.UseCases.ReceiveDataFromDevice;

namespace Spark.InterfaceAdapters.Consumers;

public class DeviceDataReceivedConsumer : IConsumer<DeviceDataReceived>
{
    public async Task ConsumeAsync(DeviceDataReceived message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"CONSUMER: Received data from device {message.DeviceId}");
    }
}
