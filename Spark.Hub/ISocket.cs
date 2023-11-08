using Spark.InterfaceAdapters.Gateways;
using System.Net;

namespace Spark.Hub;

public interface ISocket : IConnection
{
    void Bind(IPEndPoint endPoint); // TODO: IPEndPoint or just EndPoint?
    void Listen(int backlog);
    Task<ISocket> AcceptAsync(CancellationToken cancellationToken);
}

