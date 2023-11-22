using Spark.Entities;
using Spark.UseCases;

namespace Spark.Hub;

public interface IConnectionFactory<TDeviceData> where TDeviceData : IDeviceData
{
    IUninitializedConnection<TDeviceData> Create(ISocket socket);
}

