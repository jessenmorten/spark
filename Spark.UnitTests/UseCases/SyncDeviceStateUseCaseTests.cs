using NSubstitute;
using Spark.Entities;
using Spark.UseCases;
using Spark.UseCases.SyncDeviceState;

namespace Spark.UnitTests.UseCases;

public class SyncDeviceStateUseCaseTests
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly SyncDeviceStateUseCase<ILightBulb, ILightBulbData> _useCase;
    private readonly IRepository<ILightBulb, ILightBulbData> _repoMock;

    public SyncDeviceStateUseCaseTests()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _repoMock = Substitute.For<IRepository<ILightBulb, ILightBulbData>>();
        _useCase = new SyncDeviceStateUseCase<ILightBulb, ILightBulbData>(_repoMock);
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
}
