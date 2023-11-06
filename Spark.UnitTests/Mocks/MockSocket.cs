using System.Net;
using System.Net.Sockets;

namespace Spark.UnitTests.Mocks;

public class MockSocket : ISocket
{
    public List<BindCall> BindCalls { get; } = new();
    public List<ListenCall> ListenCalls { get; } = new();
    public List<AcceptCall> AcceptCalls { get; } = new();

    public void Bind(IPEndPoint endPoint)
    {
        BindCalls.Add(new(endPoint));
    }

    public void Listen(int backlog)
    {
        ListenCalls.Add(new(backlog));
    }

    public Task<ISocket> AcceptAsync(CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<ISocket>();
        AcceptCalls.Add(new(cancellationToken, tcs));
        return tcs.Task;
    }

    public record BindCall(IPEndPoint EndPoint);
    public record ListenCall(int Backlog);
    public record AcceptCall(CancellationToken CancellationToken, TaskCompletionSource<ISocket> TaskCompletionSource);
}

