using System;
using Spark.Entities;

namespace Spark.UseCases.SendDataToDevice;

public class SendDataToDevice<TDeviceData> : IUseCase<TDeviceData, TDeviceData> where TDeviceData : IDeviceData
{
    private readonly IConnectionManager<TDeviceData> _connectionManager;

    public SendDataToDevice(IConnectionManager<TDeviceData> connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task<TDeviceData> ExecuteAsync(TDeviceData request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        if (_connectionManager.TryGet(request.Id, out var connection))
        {
            await connection.UpdateAsync(request, cancellationToken);
        }

        return request;
    }
}
