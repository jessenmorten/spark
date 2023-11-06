using System.Net;
using System.Net.Sockets;
using Spark.UnitTests.Mocks;

namespace Spark.UnitTests;

public class ServerTests
{
    private readonly ServerOptions _options;
    private readonly Server _server;
    private readonly MockConnectionManager _connectionManager;
    private readonly MockConnectionFactory _connectionFactory;
    private readonly MockSocketFactory _socketFactory;

    public ServerTests()
    {
        _options = new ServerOptions
        {
            EndPoint = IPEndPoint.Parse("127.0.0.1:8080"),
            Backlog = 10,
            ConnectionType = (ConnectionType)12345
        };
        _socketFactory = new MockSocketFactory();
        _connectionFactory = new MockConnectionFactory();
        _connectionManager = new MockConnectionManager();
        _server = new Server(
            _options,
            _socketFactory,
            _connectionFactory,
            _connectionManager);
    }

    [Fact]
    public void StartThrowsWhenAlreadyStarted()
    {
        _server.Start();

        var action = () => _server.Start();

        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Server is already started", exception.Message);
    }

    [Fact]
    public void StopThrowsWhenNotStarted()
    {
        var action = () => _server.Stop();

        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Server is not started", exception.Message);
    }

    [Fact]
    public void StartUsesServerOptions()
    {
        _server.Start();

        var createCall = Assert.Single(_socketFactory.CreateCalls);
        Assert.Equal(_options.EndPoint.AddressFamily, createCall.AddressFamily);
        Assert.Equal(SocketType.Stream, createCall.SocketType);
        Assert.Equal(ProtocolType.Tcp, createCall.ProtocolType);
        var mockSocket = Assert.IsType<MockSocket>(createCall.Socket);
        var bindCall = Assert.Single(mockSocket.BindCalls);
        Assert.Equal(_options.EndPoint, bindCall.EndPoint);
        var listenCall = Assert.Single(mockSocket.ListenCalls);
        Assert.Equal(_options.Backlog, listenCall.Backlog);
    }

    [Fact]
    public void AcceptsClientSockets()
    {
        _server.Start();

        var createSocketCall = Assert.Single(_socketFactory.CreateCalls);
        var mockSocket = Assert.IsType<MockSocket>(createSocketCall.Socket);
        var acceptCall = Assert.Single(mockSocket.AcceptCalls);
        var mockClientSocket = new MockSocket();
        acceptCall.TaskCompletionSource.SetResult(mockClientSocket);
        var createConnectionCall = Assert.Single(_connectionFactory.CreateCalls);
        Assert.Equal(mockClientSocket, createConnectionCall.Socket);
        Assert.Equal(_options.ConnectionType, createConnectionCall.ConnectionType);
        var addCall = Assert.Single(_connectionManager.AddCalls);
        Assert.Equal(createConnectionCall.Connection, addCall.Connection);
    }

    [Fact]
    public void AcceptHandlesExceptions()
    {
        _server.Start();

        var createCall = Assert.Single(_socketFactory.CreateCalls);
        var mockSocket = Assert.IsType<MockSocket>(createCall.Socket);
        var acceptCall = Assert.Single(mockSocket.AcceptCalls);
        acceptCall.TaskCompletionSource.SetException(new Exception());
        Assert.Empty(_connectionManager.AddCalls);
        Assert.Equal(2, mockSocket.AcceptCalls.Count);
        acceptCall = mockSocket.AcceptCalls[1];
        acceptCall.TaskCompletionSource.SetResult(new MockSocket());
        Assert.Single(_connectionManager.AddCalls);
    }

    [Fact]
    public void StopCancelsAcceptLoop()
    {
        _server.Start();
        _server.Stop();

        var createCall = Assert.Single(_socketFactory.CreateCalls);
        var mockSocket = Assert.IsType<MockSocket>(createCall.Socket);
        var acceptCall = Assert.Single(mockSocket.AcceptCalls);
        acceptCall.TaskCompletionSource.SetResult(new MockSocket());
        Assert.Empty(_connectionManager.AddCalls);
    }
}

// TODO: Refactor: move to appropriate packages, don't know where yet
public interface IConnectionManager
{
    void Add(IConnection connection);
}

public enum ConnectionType
{
}

public interface IConnection
{
}

public interface ISocket
{
    void Bind(IPEndPoint endPoint); // TODO: IPEndPoint or just EndPoint?
    void Listen(int backlog);
    Task<ISocket> AcceptAsync(CancellationToken cancellationToken);
}

public interface ISocketFactory
{
    ISocket Create(
        AddressFamily addressFamily,
        SocketType socketType,
        ProtocolType protocolType);
}

public interface IConnectionFactory
{
    IConnection Create(ISocket socket, ConnectionType connectionType);
}

public class ServerOptions
{
    public required IPEndPoint EndPoint { get; init; }
    public required int Backlog { get; init; }
    public required ConnectionType ConnectionType { get; init; }
}

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

