using Spark.Entities;

namespace Spark.UseCases;

public interface IRepository<TEntity, TEntityData> where TEntity : TEntityData where TEntityData : IEntityData
{
    Task UpdateAsync(TEntityData entity, CancellationToken cancellationToken);
}
