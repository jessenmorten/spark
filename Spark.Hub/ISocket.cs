using System.Net;

namespace Spark.Hub;

public interface ISocket
{
    void Bind(EndPoint endPoint);
    void Listen(int backlog);
    Task<ISocket> AcceptAsync(CancellationToken cancellationToken);
}

