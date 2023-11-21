using System.Net.Sockets;
using Spark.Entities;

namespace Spark.InterfaceAdapters.Gateways;

public interface IConnection<TDeviceData> where TDeviceData : IDeviceData
{
    string Id { get; }
    Task UpdateAsync(TDeviceData deviceData);
}

