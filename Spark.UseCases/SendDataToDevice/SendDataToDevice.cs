using System;
using Spark.Entities;

namespace Spark.UseCases.SendDataToDevice;

public class SendDataToDevice<TDeviceData> where TDeviceData : IDeviceData
{
    private readonly IConnectionManager<TDeviceData> _connectionManager;

    public SendDataToDevice(IConnectionManager<TDeviceData> connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task ExecuteAsync(TDeviceData deviceData, CancellationToken cancellationToken)
    {
        if (_connectionManager.TryGet(deviceData.Id, out var connection))
        {
            await connection.UpdateAsync(deviceData, cancellationToken);
        }
    }
}
