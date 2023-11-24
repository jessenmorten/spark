using Spark.Entities;
using Spark.UseCases;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Spark.Hub;

public class ConnectionManager<TDeviceData> : IConnectionManager<TDeviceData> where TDeviceData : IDeviceData
{
    private readonly ConcurrentDictionary<string, IConnection<TDeviceData>> _connections = new();

    public int Count => _connections.Count;

    public void Add(IConnection<TDeviceData> connection)
    {
        _ = connection ?? throw new ArgumentNullException(nameof(connection));

        if (_connections.TryRemove(connection.DeviceId, out var oldConnection))
        {
            oldConnection.Close();
        }

        _connections[connection.DeviceId] = connection;
    }

    public bool TryGet(string deviceId, [NotNullWhen(true)] out IConnection<TDeviceData>? connection)
    {
        var success = _connections.TryGetValue(deviceId, out var conn);
        connection = conn;
        return success;
    }
}
