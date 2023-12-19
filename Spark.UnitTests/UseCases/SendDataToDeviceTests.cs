using System.Text;
using Spark.Entities;
using Spark.Entities.LightBulb;
using Spark.UseCases;
using Spark.UseCases.SendDataToDevice;
using Spark.Hub;

namespace Spark.UnitTests.UseCases;

public class SendDataToDeviceTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly IUseCase<ILightBulbData, ILightBulbData> _useCase;
    private readonly IConnectionManager<ILightBulbData> _connectionManager;

    public SendDataToDeviceTests()
    {
        var cts = new CancellationTokenSource(1000);
        _cancellationToken = cts.Token;
        _connectionManager = new ConnectionManager<ILightBulbData>();
        _useCase = new SendDataToDevice<ILightBulbData>(_connectionManager);
        _connectionManager.Start();
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
        Assert.Equal("Value cannot be null. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public async Task DoesNotThrowWhenConnectionIsNull()
    {
        // Arrange
        var deviceData = Substitute.For<ILightBulbData>();
        deviceData.Id.Returns("device-id");

        // Act
        var action = () => _useCase.ExecuteAsync(deviceData, CancellationToken.None);

        // Assert
        await action();
    }

    [Fact]
    public async Task SendsDataToConnection()
    {
        // Arrange
        var deviceData = Substitute.For<ILightBulbData>();
        deviceData.Id.Returns("device-id");
        var connection = Substitute.For<IConnection<ILightBulbData>>();
        connection.DeviceId.Returns("device-id");
        _connectionManager.Add(connection);

        // Act
        await _useCase.ExecuteAsync(deviceData, _cancellationToken);

        // Assert
        await connection.Received(1).UpdateAsync(deviceData, _cancellationToken);
    }
}
