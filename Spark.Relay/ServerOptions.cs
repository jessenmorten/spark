using System.Net;

namespace Spark.Relay;

public class ServerOptions
{
    public required IPEndPoint EndPoint { get; init; }
    public required int Backlog { get; init; }
    public required ConnectionType ConnectionType { get; init; }
}

