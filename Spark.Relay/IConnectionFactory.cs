namespace Spark.Relay;

public interface IConnectionFactory
{
    IConnection Create(ISocket socket, ConnectionType connectionType);
}

