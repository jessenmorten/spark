using Spark.InterfaceAdapters.Gateways;
using Spark.Entities;
using System.Net.Sockets;

namespace Spark.Hub;

public interface IConnectionFactory<TDeviceData> where TDeviceData : IDeviceData
{
    IConnection<TDeviceData> Create(ISocket socket);
}

public class Server<TDeviceData> where TDeviceData : IDeviceData
{
    private readonly object _lock;
    private readonly ServerOptions _options;
    private readonly ISocketFactory _socketFactory;
    private readonly IConnectionFactory<TDeviceData> _connectionFactory;
    private readonly IConnectionManager<TDeviceData> _connectionManager;
    private CancellationTokenSource? _cts;

    public Server(
        ServerOptions options,
        ISocketFactory socketFactory,
        IConnectionFactory<TDeviceData> connectionFactory,
        IConnectionManager<TDeviceData> connectionManager)
    {
        _lock = new();
        _options = options;
        _socketFactory = socketFactory;
        _connectionFactory = connectionFactory;
        _connectionManager = connectionManager;
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

        var cancellationToken = _cts.Token;

        Task.Factory.StartNew(
            () => AcceptLoop(socket, cancellationToken),
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
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

                var connection = _connectionFactory.Create(client);
                _connectionManager.Add(connection);
            }
            catch
            {
                // ignore
            }
        }
    }
}

