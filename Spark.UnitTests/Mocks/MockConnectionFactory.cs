namespace Spark.UnitTests.Mocks;

public class MockConnectionFactory : IConnectionFactory
{
    public List<CreateCall> CreateCalls { get; } = new();

    public IConnection Create(ISocket socket, ConnectionType connectionType)
    {
        var connection = new MockConnection();
        CreateCalls.Add(new(socket, connectionType, connection));
        return connection;
    }

    public record CreateCall(
        ISocket Socket,
        ConnectionType ConnectionType,
        IConnection Connection);
}

