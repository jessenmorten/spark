using Spark.Entities;
using Spark.UseCases;

namespace Spark.Hub;

public class ConnectionFactory<TDeviceData> : IConnectionFactory<TDeviceData> where TDeviceData : IDeviceData
{
    private readonly Func<ISocket, IUninitializedConnection<TDeviceData>> _create;

    public ConnectionFactory(Func<ISocket, IUninitializedConnection<TDeviceData>> create)
    {
        _create = create;
    }

    public IUninitializedConnection<TDeviceData> Create(ISocket socket)
    {
        return _create(socket);
    }
}
