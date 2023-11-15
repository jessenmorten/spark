using System.Net;

namespace Spark.Hub;

public class ServerOptions
{
    public required EndPoint EndPoint { get; init; }
    public required int Backlog { get; init; }
}

