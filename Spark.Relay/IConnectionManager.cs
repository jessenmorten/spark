namespace Spark.Relay;

public interface IConnectionManager
{
    int Count { get; }
    void Add(IConnection connection);
    bool TryGet(string connectionId, out IConnection? connection);
}

