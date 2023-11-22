using Spark.Entities;

namespace Spark.UseCases;

public interface IUninitializedConnection<TDeviceData> where TDeviceData : IDeviceData
{
    Task<IConnection<TDeviceData>> InitializeAsync(CancellationToken cancellationToken);
}

