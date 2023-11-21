using System.Diagnostics.CodeAnalysis;
using Spark.Entities;

namespace Spark.InterfaceAdapters.Gateways;

public interface IConnectionManager<TDeviceData> where TDeviceData : IDeviceData
{
    int Count { get; }
    void Add(IConnection<TDeviceData> connection);
    bool TryGet(string connectionId, [NotNullWhen(returnValue: true)] out IConnection<TDeviceData>? connection);
}

