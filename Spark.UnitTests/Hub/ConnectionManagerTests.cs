using Spark.Entities;
using Spark.Hub;
using Spark.InterfaceAdapters.Gateways;

namespace Spark.UnitTests.Hub;

public class ConnectionManagerTests
{
    private readonly ConnectionManager<ILightBulbData> _connectionManager;

    public ConnectionManagerTests()
    {
        _connectionManager = new ConnectionManager<ILightBulbData>();
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
        IConnection<ILightBulbData> connection = null!;

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
        var first = Substitute.For<IConnection<ILightBulbData>>();
        first.Id.Returns("connection-id");
        var second = Substitute.For<IConnection<ILightBulbData>>();
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
        var first = Substitute.For<IConnection<ILightBulbData>>();
        first.Id.Returns("1");
        var second = Substitute.For<IConnection<ILightBulbData>>();
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
        var connection = Substitute.For<IConnection<ILightBulbData>>();
        connection.Id.Returns("1");
        _connectionManager.Add(connection);

        // Act
        var found = _connectionManager.TryGet(connection.Id, out var outVal);

        // Assert
        Assert.True(found);
        Assert.NotNull(outVal);
        Assert.Equal(connection, outVal);
    }
}
