namespace Tinkerforge.Worker.Notifier;

public class NotifierService(ILogger<NotifierService> logger, IPConnection ipConnection) : IFeatureService
{
    private const string Uid = "Wcg";

    public void ExecuteService()
    {
        var ptc = new BrickletPTCV2(Uid, ipConnection);

        // var temperature = ptc.GetTemperature();
        logger.LogInformation("Temperature: {Temperature} °C", 3000 / 100);
    }
}