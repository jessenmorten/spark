using Spark.Hub;
using Spark.UnitTests.Mocks;

namespace Spark.UnitTests.Hub;

public class ConnectionManagerTests
{
    private readonly ConnectionManager _connectionManager;

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
        var first = new MockSocket("connection-id");
        var second = new MockSocket("connection-id");
        _connectionManager.Add(first);
        var action = () => _connectionManager.Add(second);
        var message = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Connection already added, id: connection-id", message.Message);
    }

    [Fact]
    public void AddIncreasesCount()
    {
        _connectionManager.Add(new MockSocket(Guid.NewGuid().ToString()));
        _connectionManager.Add(new MockSocket(Guid.NewGuid().ToString()));
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
        var connection = new MockSocket("connection-id");
        _connectionManager.Add(connection);
        var found = _connectionManager.TryGet(connection.Id, out var outVal);
        Assert.True(found);
        Assert.NotNull(outVal);
        Assert.Equal(connection, outVal);
    }
}
