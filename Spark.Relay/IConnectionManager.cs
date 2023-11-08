using System.Diagnostics.CodeAnalysis;

namespace Spark.Relay;

public interface IConnectionManager
{
    int Count { get; }
    void Add(IConnection connection);
    bool TryGet(string connectionId, [NotNullWhen(returnValue: true)] out IConnection? connection);
}

