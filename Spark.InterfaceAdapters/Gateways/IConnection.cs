using System.Net.Sockets;
using Spark.Entities;

namespace Spark.InterfaceAdapters.Gateways;

public interface IConnection<TEntityData> where TEntityData : IEntityData
{
    string Id { get; }
    Task<int> ReceiveAsync(ArraySegment<byte> data);
    Task<int> SendAsync(ArraySegment<byte> data);
    Task UpdateAsync(TEntityData entityData);
}

