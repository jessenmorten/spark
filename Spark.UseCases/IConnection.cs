using Spark.Entities;

namespace Spark.UseCases;

public interface IConnection<TDeviceData> where TDeviceData : IDeviceData
{
    string DeviceId { get; }
    Task UpdateAsync(TDeviceData deviceData, CancellationToken cancellationToken);
}
