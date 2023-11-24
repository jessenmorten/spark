using System.Net;
using System.Net.Sockets;

namespace Spark.Hub;

public class SocketFactory : ISocketFactory
{
    public ISocket Create(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
    {
        var socket = new Socket(addressFamily, socketType, protocolType);
        return new SocketWrapper(socket);
    }
}
