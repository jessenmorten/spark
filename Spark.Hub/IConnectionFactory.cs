using Spark.InterfaceAdapters.Gateways;
using Spark.Entities;

namespace Spark.Hub;

public interface IConnectionFactory<TDeviceData> where TDeviceData : IDeviceData
{
    IConnection<TDeviceData> Create(ISocket socket);
}

