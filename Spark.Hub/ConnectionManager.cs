using Spark.InterfaceAdapters.Gateways;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Spark.Hub;

public class ConnectionManager : IConnectionManager
{
    private readonly ConcurrentDictionary<string, IConnection> _connections = new();

    public int Count => _connections.Count;

    public void Add(IConnection connection)
    {
        _ = connection ?? throw new ArgumentNullException(nameof(connection));

        if (!_connections.TryAdd(connection.Id, connection))
        {
            throw new InvalidOperationException($"Connection already added, id: {connection.Id}");
        }
    }

    public bool TryGet(string connectionId, [NotNullWhen(true)] out IConnection? connection)
    {
        var success = _connections.TryGetValue(connectionId, out var conn);
        connection = conn;
        return success;
    }
}
