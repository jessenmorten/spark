using System.Net;
using System.Net.Sockets;

namespace Spark.Hub;

public class SocketWrapper : ISocket
{
    private Socket? _socket;

    public SocketWrapper(Socket socket)
    {
        _socket = socket ?? throw new ArgumentNullException(nameof(socket));
    }

    public void Bind(EndPoint endPoint)
    {
        GetSocket().Bind(endPoint);
    }

    public void Listen(int backlog)
    {
        GetSocket().Listen(backlog);
    }

    public void Close()
    {
        GetSocket().Close();
        GetSocket().Dispose();
        _socket = null;
    }

    public async Task<ISocket> AcceptAsync(CancellationToken cancellationToken)
    {
        var socket = await GetSocket().AcceptAsync();
        return new SocketWrapper(socket);
    }

    private Socket GetSocket()
    {
        if (_socket == null)
        {
            throw new ObjectDisposedException(nameof(_socket));
        }

        return _socket;
    }
}
