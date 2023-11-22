using System.Net;
using System.Net.Sockets;
using Spark.Entities;
using Spark.Entities.LightBulb;
using Spark.Hub;
using Spark.UseCases;

namespace Spark.UnitTests.Hub;

public class ServerTests
{
    private readonly TimeSpan _acceptDelay = TimeSpan.FromMilliseconds(25);
    private readonly ServerOptions _options;
    private readonly Server<ILightBulbData> _server;
    private readonly IConnectionManager<ILightBulbData> _connectionManager;
    private readonly IConnectionFactory<ILightBulbData> _connectionFactory;
    private readonly ISocketFactory _socketFactory;

    public ServerTests()
    {
        _options = new ServerOptions
        {
            EndPoint = IPEndPoint.Parse("127.0.0.1:8080"),
            Backlog = 42
        };
        _socketFactory = Substitute.For<ISocketFactory>();
        _connectionManager = Substitute.For<IConnectionManager<ILightBulbData>>();
        _connectionFactory = Substitute.For<IConnectionFactory<ILightBulbData>>();
        _server = new Server<ILightBulbData>(
            _options,
            _socketFactory,
            _connectionFactory,
            _connectionManager);
    }

    [Fact]
    public void StartThrowsWhenAlreadyStarted()
    {
        // Arrange
        _server.Start();

        // Act
        var action = () => _server.Start();

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Server is already started", exception.Message);
    }

    [Fact]
    public void StopThrowsWhenNotStarted()
    {
        // Act
        var action = () => _server.Stop();

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Server is not started", exception.Message);
    }

    [Fact]
    public void StartUsesServerOptions()
    {
        // Arrange
        var socket = Substitute.For<ISocket>();

        _socketFactory
            .Create(_options.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            .Returns(socket);

        // Act
        _server.Start();

        // Assert
        _socketFactory
            .Received(1)
            .Create(_options.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket
            .Received(1)
            .Bind(_options.EndPoint);
        socket
            .Received(1)
            .Listen(_options.Backlog);
    }

    [Fact]
    public async Task AcceptsClientSockets()
    {
        // Arrange
        var serverSocket = Substitute.For<ISocket>();
        var clientSocket = Substitute.For<ISocket>();
        var uninitializedConnection = Substitute.For<IUninitializedConnection<ILightBulbData>>();
        var connection = Substitute.For<IConnection<ILightBulbData>>();

        uninitializedConnection
            .InitializeAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(connection));
        _socketFactory
            .Create(_options.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            .Returns(serverSocket);
        _connectionFactory
            .Create(clientSocket)
            .Returns(uninitializedConnection);
        serverSocket
            .AcceptAsync(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(clientSocket),
                Task.FromResult(clientSocket).Delay());

        // Act
        _server.Start();
        await Task.Delay(_acceptDelay);

        // Assert
        _connectionManager.Received(1).Add(connection);
    }

    [Fact]
    public async Task AcceptHandlesExceptions()
    {
        // Arrange
        var serverSocket = Substitute.For<ISocket>();
        var clientSocket = Substitute.For<ISocket>();
        var uninitializedConnection = Substitute.For<IUninitializedConnection<ILightBulbData>>();
        var connection = Substitute.For<IConnection<ILightBulbData>>();

        uninitializedConnection
            .InitializeAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(connection));
        _socketFactory
            .Create(_options.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            .Returns(serverSocket);
        _connectionFactory
            .Create(clientSocket)
            .Returns(uninitializedConnection);
        serverSocket
            .AcceptAsync(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromException(new Exception()),
                Task.FromResult(clientSocket),
                Task.FromResult(clientSocket).Delay());

        // Act
        _server.Start();
        await Task.Delay(_acceptDelay);

        // Assert
        _connectionManager.Received(1).Add(connection);
    }

    [Fact]
    public void StopCancelsAcceptLoop()
    {
        // Arrange
        var serverSocket = Substitute.For<ISocket>();
        var clientSocket = Substitute.For<ISocket>();

        _socketFactory
            .Create(_options.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            .Returns(serverSocket);
        serverSocket
            .AcceptAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(clientSocket))
            .AndDoes((x) => _server.Stop());

        // Act
        _server.Start();

        // Assert
        _connectionManager.Received(0).Add(Arg.Any<IConnection<ILightBulbData>>());
    }
}
