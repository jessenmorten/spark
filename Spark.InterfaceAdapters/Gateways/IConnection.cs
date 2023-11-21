using System.Net.Sockets;
using Spark.Entities;

namespace Spark.InterfaceAdapters.Gateways;

public interface IConnection<TEntityData> where TEntityData : IEntityData
{
    string Id { get; }
    Task UpdateAsync(TEntityData entityData);
}

