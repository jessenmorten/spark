using Spark.Hub;
using NSubstitute;
using Spark.InterfaceAdapters.Gateways;

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
        // Arrange
        IConnection connection = null!;

        // Act
        var action = () => _connectionManager.Add(connection);

        // Assert
        var message = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal("connection", message.ParamName);
    }

    [Fact]
    public void AddThrowsWhenAlreadyAdded()
    {
        // Arrange
        var first = Substitute.For<ISocket>();
        first.Id.Returns("connection-id");
        var second = Substitute.For<ISocket>();
        second.Id.Returns("connection-id");
        _connectionManager.Add(first);

        // Act
        var action = () => _connectionManager.Add(second);

        // Assert
        var message = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Connection already added, id: connection-id", message.Message);
    }

    [Fact]
    public void AddIncreasesCount()
    {
        // Arrange
        var first = Substitute.For<ISocket>();
        first.Id.Returns("1");
        var second = Substitute.For<ISocket>();
        second.Id.Returns("2");

        // Act
        _connectionManager.Add(first);
        _connectionManager.Add(second);
        
        // Assert
        Assert.Equal(2, _connectionManager.Count);
    }

    [Fact]
    public void TryGetReturnsFalse()
    {
        // Arrange
        var id = "connection-id";

        // Act
        var found = _connectionManager.TryGet(id, out var outVal);

        // Assert
        Assert.False(found);
        Assert.Null(outVal);
    }

    [Fact]
    public void TryGetReturnsTrue()
    {
        // Arrange
        var socket = Substitute.For<ISocket>();
        socket.Id.Returns("1");
        _connectionManager.Add(socket);

        // Act
        var found = _connectionManager.TryGet(socket.Id, out var outVal);

        // Assert
        Assert.True(found);
        Assert.NotNull(outVal);
        Assert.Equal(socket, outVal);
    }
}
