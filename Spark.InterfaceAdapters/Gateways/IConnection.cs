using System.Net.Sockets;

namespace Spark.InterfaceAdapters.Gateways;

public interface IConnection
{
    string Id { get; }
    Task<int> SendAsync(ArraySegment<byte> data);
    Task<int> ReceiveAsync(ArraySegment<byte> data);
}

