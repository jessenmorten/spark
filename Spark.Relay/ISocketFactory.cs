using System.Net.Sockets;

namespace Spark.Relay;

public interface ISocketFactory
{
    ISocket Create(
        AddressFamily addressFamily,
        SocketType socketType,
        ProtocolType protocolType);
}

