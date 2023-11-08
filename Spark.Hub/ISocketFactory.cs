using System.Net.Sockets;

namespace Spark.Hub;

public interface ISocketFactory
{
    ISocket Create(
        AddressFamily addressFamily,
        SocketType socketType,
        ProtocolType protocolType);
}

