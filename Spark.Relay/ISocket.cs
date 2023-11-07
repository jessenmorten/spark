using System.Net;

namespace Spark.Relay;

public interface ISocket
{
    void Bind(IPEndPoint endPoint); // TODO: IPEndPoint or just EndPoint?
    void Listen(int backlog);
    Task<ISocket> AcceptAsync(CancellationToken cancellationToken);
}

