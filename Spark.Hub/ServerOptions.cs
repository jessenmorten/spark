using System.Net;

namespace Spark.Hub;

public class ServerOptions
{
    public required IPEndPoint EndPoint { get; init; }
    public required int Backlog { get; init; }
}

