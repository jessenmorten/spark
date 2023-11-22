using Spark.Entities;

namespace Spark.UseCases;

public record DeviceDataChanged<TDeviceData>(TDeviceData DeviceData) where TDeviceData : IDeviceData;
