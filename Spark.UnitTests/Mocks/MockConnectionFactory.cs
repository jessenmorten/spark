namespace Spark.UnitTests.Mocks;

public class MockConnectionFactory : IConnectionFactory
{
    public List<CreateCall> CreateCalls { get; } = new();

    public IConnection Create(ISocket socket, ConnectionType connectionType)
    {
        var id = Guid.NewGuid().ToString();
        var connection = new MockConnection(id);
        CreateCalls.Add(new(socket, connectionType, connection));
        return connection;
    }

    public record CreateCall(
        ISocket Socket,
        ConnectionType ConnectionType,
        IConnection Connection);
}

