using System.Net.Sockets;

namespace Spark.Relay;

public class Server
{
    private readonly object _lock;
    private readonly ServerOptions _options;
    private readonly ISocketFactory _socketFactory;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IConnectionManager _connectionManager;
    private CancellationTokenSource? _cts;

    public Server(
        ServerOptions options,
        ISocketFactory socketFactory,
        IConnectionFactory connectionFactory,
        IConnectionManager socketList)
    {
        _lock = new();
        _options = options;
        _socketFactory = socketFactory;
        _connectionFactory = connectionFactory;
        _connectionManager = socketList;
    }

    public void Start()
    {
        lock (_lock)
        {
            if (_cts is not null)
            {
                throw new InvalidOperationException("Server is already started");
            }

            _cts = new();
        }

        var socket = _socketFactory.Create(
            _options.EndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        socket.Bind(_options.EndPoint);
        socket.Listen(_options.Backlog);
        _ = AcceptLoop(socket, _cts.Token);
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (_cts is null)
            {
                throw new InvalidOperationException("Server is not started");
            }

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private async Task AcceptLoop(ISocket socket, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var client = await socket.AcceptAsync(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var connection = _connectionFactory.Create(client, _options.ConnectionType);
                _connectionManager.Add(connection);
            }
            catch
            {
                // ignore
            }
        }
    }
}

