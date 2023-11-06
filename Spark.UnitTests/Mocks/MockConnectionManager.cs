namespace Spark.UnitTests.Mocks;

public class MockConnectionManager : IConnectionManager
{
    public List<AddCall> AddCalls { get; } = new();

    public void Add(IConnection connection)
    {
        AddCalls.Add(new(connection));
    }

    public record AddCall(IConnection Connection);
}

