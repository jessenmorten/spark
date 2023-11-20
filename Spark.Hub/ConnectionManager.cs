using Spark.Entities;
using Spark.InterfaceAdapters.Gateways;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Spark.Hub;

public class ConnectionManager<TEntityData> : IConnectionManager<TEntityData> where TEntityData : IEntityData
{
    private readonly ConcurrentDictionary<string, IConnection<TEntityData>> _connections = new();

    public int Count => _connections.Count;

    public void Add(IConnection<TEntityData> connection)
    {
        _ = connection ?? throw new ArgumentNullException(nameof(connection));

        if (!_connections.TryAdd(connection.Id, connection))
        {
            throw new InvalidOperationException($"Connection already added, id: {connection.Id}");
        }
    }

    public bool TryGet(string connectionId, [NotNullWhen(true)] out IConnection<TEntityData>? connection)
    {
        var success = _connections.TryGetValue(connectionId, out var conn);
        connection = conn;
        return success;
    }
}
