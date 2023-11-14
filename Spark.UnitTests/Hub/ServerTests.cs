using System.Net;
using System.Net.Sockets;
using NSubstitute;
using Spark.Hub;
using Spark.InterfaceAdapters.Gateways;

namespace Spark.UnitTests.Hub;

public class ServerTests
{
    private readonly TimeSpan _acceptDelay = TimeSpan.FromMilliseconds(25);
    private readonly ServerOptions _options;
    private readonly Server _server;
    private readonly IConnectionManager _connectionManager;
    private readonly ISocketFactory _socketFactory;

    public ServerTests()
    {
        _options = new ServerOptions
        {
            EndPoint = IPEndPoint.Parse("127.0.0.1:8080"),
            Backlog = 42
        };
        _socketFactory = Substitute.For<ISocketFactory>();
        _connectionManager = Substitute.For<IConnectionManager>();
        _server = new Server(
            _options,
            _socketFactory,
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

        _socketFactory
            .Create(_options.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            .Returns(serverSocket);
        serverSocket
            .AcceptAsync(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(clientSocket),
                Task.FromResult(clientSocket).Delay());

        // Act
        _server.Start();
        await Task.Delay(_acceptDelay);

        // Assert
        _connectionManager.Received(1).Add(clientSocket);
    }

    [Fact]
    public async Task AcceptHandlesExceptions()
    {
        // Arrange
        var serverSocket = Substitute.For<ISocket>();
        var clientSocket = Substitute.For<ISocket>();

        _socketFactory
            .Create(_options.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            .Returns(serverSocket);
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
        _connectionManager.Received(1).Add(clientSocket);
    }

    [Fact]
    public async Task StopCancelsAcceptLoop()
    {
        // Arrange
        var serverSocket = Substitute.For<ISocket>();
        var clientSocket = Substitute.For<ISocket>();

        _socketFactory
            .Create(_options.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            .Returns(serverSocket);
        serverSocket
            .AcceptAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(clientSocket));

        // Act
        _server.Start();
        _server.Stop();
        await Task.Delay(_acceptDelay);

        // Assert
        _connectionManager.Received(0).Add(Arg.Any<IConnection>());
    }
}
