using System.Net;
using System.Net.Sockets;

namespace Spark.Hub;

public class SocketWrapper : ISocket
{
    private readonly Socket _socket;

    public SocketWrapper(Socket socket)
    {
        _socket = socket;
    }

    public void Bind(EndPoint endPoint)
    {
        _socket.Bind(endPoint);
    }

    public void Listen(int backlog)
    {
        _socket.Listen(backlog);
    }

    public async Task<ISocket> AcceptAsync(CancellationToken cancellationToken)
    {
        var socket = await _socket.AcceptAsync();
        return new SocketWrapper(socket);
    }
}
