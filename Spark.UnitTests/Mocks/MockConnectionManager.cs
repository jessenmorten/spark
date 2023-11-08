using Spark.InterfaceAdapters.Gateways;

namespace Spark.UnitTests.Mocks;

public class MockConnectionManager : IConnectionManager
{
    public List<AddCall> AddCalls { get; } = new();

    public int Count => throw new NotImplementedException();

    public void Add(IConnection connection)
    {
        AddCalls.Add(new(connection));
    }

    public bool TryGet(string connectionId, out IConnection connection)
    {
        throw new NotImplementedException();
    }

    public record AddCall(IConnection Connection);
}

