using Spark.Entities;

namespace Spark.UseCases;

public interface IConnection<TDeviceData> where TDeviceData : IDeviceData
{
    bool Healthy { get; }
    string DeviceId { get; }
    Task UpdateAsync(TDeviceData deviceData, CancellationToken cancellationToken);
    void Close();
}
