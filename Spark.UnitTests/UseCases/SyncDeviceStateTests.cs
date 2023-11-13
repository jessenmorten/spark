using NSubstitute;
using System.Text;
using Spark.Entities;
using Spark.UseCases;
using Spark.UseCases.SyncDeviceState;

namespace Spark.UnitTests.UseCases;

public class SyncDeviceStateTests
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly SyncDeviceState<ILightBulb, ILightBulbData> _useCase;
    private readonly IRepository<ILightBulb, ILightBulbData> _repoMock;
    private readonly IMessageBroker _messageBrokerMock;

    public SyncDeviceStateTests()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _repoMock = Substitute.For<IRepository<ILightBulb, ILightBulbData>>();
        _messageBrokerMock = Substitute.For<IMessageBroker>();
        _useCase = new SyncDeviceState<ILightBulb, ILightBulbData>(
            _repoMock,
            _messageBrokerMock);
    }

    [Fact]
    public async Task ThrowsIfEntityIsNull()
    {
        // Arrange
        ILightBulb entity = null!;

        // Act
        var action = async () => await _useCase.ExecuteAsync(entity, _cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(action);
        Assert.Equal("Value cannot be null. (Parameter 'entity')", exception.Message);
    }

    [Fact]
    public async Task UpdatesEntityInRepository()
    {
        // Arrange
        var entity = new LightBulb(Guid.NewGuid(), on: false);

        // Act
        await _useCase.ExecuteAsync(entity, _cancellationToken);

        // Assert
        await _repoMock.Received(1).UpdateAsync(entity, _cancellationToken);
    }

    [Fact]
    public async Task PublishesDeviceSyncEvent()
    {
        // Arrange
        var entity = new LightBulb(Guid.NewGuid(), on: false);

        // Act
        await _useCase.ExecuteAsync(entity, _cancellationToken);

        // Assert
        await _messageBrokerMock.Received(1).PublishAsync(Arg.Is<DeviceSyncEvent>(e => e.EntityId == entity.Id), _cancellationToken);
    }

    [Fact]
    public async Task DoesNotPublishWhenRepositoryThrows()
    {
        // Arrange
        var entity = new LightBulb(Guid.NewGuid(), on: false);
        _repoMock.UpdateAsync(entity, _cancellationToken).Returns(Task.FromException(new Exception()));

        // Act
        var action = async () => await _useCase.ExecuteAsync(entity, _cancellationToken);

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
        await _messageBrokerMock.DidNotReceive().PublishAsync(Arg.Any<DeviceSyncEvent>(), _cancellationToken);
    }
}

