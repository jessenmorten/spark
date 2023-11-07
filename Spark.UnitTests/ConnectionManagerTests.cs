using System.Collections.Concurrent;
using Spark.UnitTests.Mocks;

namespace Spark.UnitTests;

public class ConnectionManagerTests
{
    private ConnectionManager _connectionManager;

    public ConnectionManagerTests()
    {
        _connectionManager = new ConnectionManager();
    }

    [Fact]
    public void NewInstanceIsEmpty()
    {
        Assert.Equal(0, _connectionManager.Count);
    }

    [Fact]
    public void AddThrowsWhenConnectionIsNull()
    {
        var action = () => _connectionManager.Add(null!);
        var message = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal("connection", message.ParamName);
    }

    [Fact]
    public void AddThrowsWhenAlreadyAdded()
    {
        var first = new MockConnection("connection-id");
        var second = new MockConnection("connection-id");
        _connectionManager.Add(first);
        var action = () => _connectionManager.Add(second);
        var message = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Connection already added, id: connection-id", message.Message);
    }

    [Fact]
    public void AddIncreasesCount()
    {
        _connectionManager.Add(new MockConnection(Guid.NewGuid().ToString()));
        _connectionManager.Add(new MockConnection(Guid.NewGuid().ToString()));
        Assert.Equal(2, _connectionManager.Count);
    }

    [Fact]
    public void TryGetReturnsFalse()
    {
        var id = "connection-id";
        var found = _connectionManager.TryGet(id, out var outVal);
        Assert.False(found);
        Assert.Null(outVal);
    }

    [Fact]
    public void TryGetReturnsTrue()
    {
        var connection = new MockConnection("connection-id");
        _connectionManager.Add(connection);
        var found = _connectionManager.TryGet(connection.Id, out var outVal);
        Assert.True(found);
        Assert.NotNull(outVal);
        Assert.Equal(connection, outVal);
    }
}

// TODO: Refactor: move to appropriate package, don't know where yet
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

    public bool TryGet(string connectionId, out IConnection? connection)
    {
        var success = _connections.TryGetValue(connectionId, out var conn);
        connection = conn;
        return success;
    }
}
