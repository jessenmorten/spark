using Spark.Entities;
using Spark.UseCases;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Spark.Hub;

public class Server<TDeviceData> where TDeviceData : IDeviceData
{
    private readonly object _lock;
    private readonly Channel<IUninitializedConnection<TDeviceData>> _channel;
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
        _channel = Channel.CreateUnbounded<IUninitializedConnection<TDeviceData>>();
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
            () => InitializeConnectionLoop(cancellationToken),
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);

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

                _channel.Writer.TryWrite(_connectionFactory.Create(client));
            }
            catch
            {
                // ignore
            }
        }
    }

    private async Task InitializeConnectionLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var uninitializedConnection = await _channel.Reader.ReadAsync(cancellationToken);

            try
            {
                var connection = await uninitializedConnection.InitializeAsync(cancellationToken);
                _connectionManager.Add(connection);
            }
            catch
            {
                // ignore
            }
        }
    }
}

