namespace Tinkerforge.Worker.NotificationService;

public class NotificationService(
    IPConnection ipConnection,
    ITelegramService telegramService,
    int temperatureThreshold,
    int humidityThreshold)
    : IFeatureService
{
    private const string TemperatureSensorUid = "Wcg";
    private const string HumiditySensorUid = "ViW";

    public void ExecuteService()
    {
        var temperatureSensor = new BrickletPTCV2(TemperatureSensorUid, ipConnection);
        var humiditySensor = new BrickletHumidityV2(HumiditySensorUid, ipConnection);

        temperatureSensor.TemperatureCallback += NotifyHighTemperature;
        temperatureSensor.SetTemperatureCallbackConfiguration(10000, false, '>', temperatureThreshold * 100, 0);

        humiditySensor.HumidityCallback += NotifyHighHumidity;
        humiditySensor.SetHumidityCallbackConfiguration(10000, false, '>', humidityThreshold * 100, 0);
    }

    private void NotifyHighTemperature(BrickletPTCV2 sender, int temperature)
    {
        telegramService.SendMessageAsync($"Warning - High Temperature: {temperature / 100}°C");
    }

    private void NotifyHighHumidity(BrickletHumidityV2 sender, int humidity)
    {
        telegramService.SendMessageAsync($"Warning - High Humidity: {humidity / 100}%");
    }
}