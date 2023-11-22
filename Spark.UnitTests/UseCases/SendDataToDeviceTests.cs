using System.Text;
using Spark.Entities;
using Spark.Entities.LightBulb;
using Spark.UseCases;
using Spark.UseCases.SendDataToDevice;
using Spark.Hub;

namespace Spark.UnitTests.UseCases;

public class SendDataToDeviceTests
{
    private readonly SendDataToDevice<ILightBulbData> _useCase;
    private readonly IConnectionManager<ILightBulbData> _connectionManager;

    public SendDataToDeviceTests()
    {
        _connectionManager = new ConnectionManager<ILightBulbData>();
        _useCase = new SendDataToDevice<ILightBulbData>(_connectionManager);
    }

    [Fact]
    public async Task DoesNotThrowWhenConnectionIsNull()
    {
        // Arrange
        var deviceData = Substitute.For<ILightBulbData>();
        deviceData.Id.Returns("device-id");

        // Act
        var action = () => _useCase.ExecuteAsync(deviceData);

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
        await _useCase.ExecuteAsync(deviceData);

        // Assert
        await connection.Received(1).UpdateAsync(deviceData);
    }
}
