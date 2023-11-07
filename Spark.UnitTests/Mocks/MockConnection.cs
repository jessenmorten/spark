using Spark.Relay;

namespace Spark.UnitTests.Mocks;

public class MockConnection : IConnection
{
    public string Id { get; }

    public MockConnection(string id)
    {
        Id = id;
    }
}

