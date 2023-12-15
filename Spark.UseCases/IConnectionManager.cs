using System.Diagnostics.CodeAnalysis;
using Spark.Entities;

namespace Spark.UseCases;

public interface IConnectionManager<TDeviceData> where TDeviceData : IDeviceData
{
    int Count { get; }
    void Start();
    void Stop();
    void Add(IConnection<TDeviceData> connection);
    bool TryGet(string deviceId, [NotNullWhen(returnValue: true)] out IConnection<TDeviceData>? connection);
}

