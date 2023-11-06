using System.Net;
using System.Net.Sockets;

namespace Spark.UnitTests.Mocks;

public class MockSocketFactory : ISocketFactory
{
    public List<CreateCall> CreateCalls { get; } = new();

    public ISocket Create(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
    {
        var socket = new MockSocket();
        CreateCalls.Add(new(addressFamily, socketType, protocolType, socket));
        return socket;
    }

    public record CreateCall(
        AddressFamily AddressFamily,
        SocketType SocketType,
        ProtocolType ProtocolType,
        ISocket Socket);
}

