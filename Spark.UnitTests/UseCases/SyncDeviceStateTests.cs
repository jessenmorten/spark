using System.Text;
using Spark.Entities;
using Spark.Entities.LightBulb;
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
    public async Task ThrowsIfDeviceIsNull()
    {
        // Arrange
        ILightBulb device = null!;

        // Act
        var action = async () => await _useCase.ExecuteAsync(device, _cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(action);
        Assert.Equal("Value cannot be null. (Parameter 'device')", exception.Message);
    }

    [Fact]
    public async Task UpdatesDeviceInRepository()
    {
        // Arrange
        var device = new LightBulb("light-id", on: false);

        // Act
        await _useCase.ExecuteAsync(device, _cancellationToken);

        // Assert
        await _repoMock.Received(1).UpdateAsync(device, _cancellationToken);
    }

    [Fact]
    public async Task PublishesDeviceSyncEvent()
    {
        // Arrange
        var device = new LightBulb("light-id", on: false);

        // Act
        await _useCase.ExecuteAsync(device, _cancellationToken);

        // Assert
        await _messageBrokerMock.Received(1).PublishAsync(Arg.Is<DeviceSyncEvent>(e => e.DeviceId == device.Id), _cancellationToken);
    }

    [Fact]
    public async Task DoesNotPublishWhenRepositoryThrows()
    {
        // Arrange
        var device = new LightBulb("light-id", on: false);
        _repoMock.UpdateAsync(device, _cancellationToken).Returns(Task.FromException(new Exception()));

        // Act
        var action = async () => await _useCase.ExecuteAsync(device, _cancellationToken);

        // Assert
        await Assert.ThrowsAsync<Exception>(action);
        await _messageBrokerMock.DidNotReceive().PublishAsync(Arg.Any<DeviceSyncEvent>(), _cancellationToken);
    }
}

