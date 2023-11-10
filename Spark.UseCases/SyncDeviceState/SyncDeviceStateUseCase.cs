using Spark.Entities;

namespace Spark.UseCases.SyncDeviceState;

public class SyncDeviceStateUseCase<TEntity, TEntityData> where TEntity : TEntityData where TEntityData : IEntityData
{
    private readonly IRepository<TEntity, TEntityData> _repository;

    public SyncDeviceStateUseCase(IRepository<TEntity, TEntityData> repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(TEntityData entity, CancellationToken cancellationToken)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        await _repository.UpdateAsync(entity, cancellationToken);
    }
}
