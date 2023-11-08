using Spark.Hub;
using System.Net;

namespace Spark.UnitTests.Mocks;

public class MockSocket : ISocket
{
    public List<BindCall> BindCalls { get; } = new();
    public List<ListenCall> ListenCalls { get; } = new();
    public List<AcceptCall> AcceptCalls { get; } = new();

    public string Id { get; }

    public MockSocket() : this(Guid.NewGuid().ToString()) { }

    public MockSocket(string id)
    {
        Id = id;
    }

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

    public Task<int> SendAsync(ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public Task<int> ReceiveAsync(ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public record BindCall(IPEndPoint EndPoint);
    public record ListenCall(int Backlog);
    public record AcceptCall(CancellationToken CancellationToken, TaskCompletionSource<ISocket> TaskCompletionSource);
}

