using System.Diagnostics.CodeAnalysis;
using Spark.Entities;

namespace Spark.InterfaceAdapters.Gateways;

public interface IConnectionManager<TEntityData> where TEntityData : IEntityData
{
    int Count { get; }
    void Add(IConnection<TEntityData> connection);
    bool TryGet(string connectionId, [NotNullWhen(returnValue: true)] out IConnection<TEntityData>? connection);
}

