using Spark.Entities;

namespace Spark.UseCases;

public interface IRepository<TDevice, TDeviceData> where TDevice : TDeviceData where TDeviceData : IDeviceData
{
    Task UpdateAsync(TDeviceData deviceData, CancellationToken cancellationToken);
}
