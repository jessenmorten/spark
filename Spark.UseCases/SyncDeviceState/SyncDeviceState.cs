using Spark.Entities;

namespace Spark.UseCases.SyncDeviceState;

public class SyncDeviceState<TEntity, TEntityData> where TEntity : TEntityData where TEntityData : IEntityData
{
    private readonly IRepository<TEntity, TEntityData> _repository;
    private readonly IMessageBroker _messageBroker;

    public SyncDeviceState(
        IRepository<TEntity, TEntityData> repository,
        IMessageBroker eventBus)
    {
        _repository = repository;
        _messageBroker = eventBus;
    }

    public async Task ExecuteAsync(TEntityData entity, CancellationToken cancellationToken)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        await _repository.UpdateAsync(entity, cancellationToken);
        var syncEvent = new DeviceSyncEvent(entity.Id);
        await _messageBroker.PublishAsync(syncEvent, cancellationToken);
    }
}
