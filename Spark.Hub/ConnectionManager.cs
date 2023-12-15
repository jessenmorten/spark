using Spark.Entities;
using Spark.UseCases;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Spark.Hub;

public class ConnectionManager<TDeviceData> : IConnectionManager<TDeviceData> where TDeviceData : IDeviceData
{
    private readonly object _lock = new();
    private CancellationTokenSource? _cts;
    private readonly ConcurrentDictionary<string, IConnection<TDeviceData>> _connections = new();

    public int Count => _connections.Count;

    public void Start()
    {
        lock (_lock)
        {
            if (_cts is not null)
            {
                throw new InvalidOperationException("ConnectionManager is already started");
            }

            _cts = new CancellationTokenSource();
        }

        var token = _cts.Token;
        Task.Factory.StartNew(
            () => CloseUnhealthyConnectionsLoop(token),
            token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (_cts is null)
            {
                throw new InvalidOperationException("ConnectionManager is not started");
            }

            _cts.Cancel();
            _cts = null;
        }
    }

    public void Add(IConnection<TDeviceData> connection)
    {
        _ = connection ?? throw new ArgumentNullException(nameof(connection));

        if (_connections.TryRemove(connection.DeviceId, out var oldConnection))
        {
            oldConnection.Close();
        }

        _connections[connection.DeviceId] = connection;
    }

    public bool TryGet(string deviceId, [NotNullWhen(true)] out IConnection<TDeviceData>? connection)
    {
        var success = _connections.TryGetValue(deviceId, out var conn);
        connection = conn;
        return success;
    }

    private async Task CloseUnhealthyConnectionsLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);

            var connections = _connections.Values.ToArray();
            foreach (var connection in connections)
            {
                if (!connection.Healthy)
                {
                    connection.Close();
                    _connections.TryRemove(connection.DeviceId, out _);
                }
            }

        }
    }
}
