namespace Tinkerforge.Worker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    private const string Host = "172.20.10.242";
    private const int Port = 4223;
    private readonly IPConnection _ipConnection = new();

    private const string Uid = "Wcg";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _ipConnection.Connect(Host, Port);
            var ptc = new BrickletPTCV2(Uid, _ipConnection);

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

                var temperature = ptc.GetTemperature();
                logger.LogInformation("Temperature: {Temperature} °C", temperature / 100);

                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
        }
        finally
        {
            _ipConnection.Disconnect();
            logger.LogInformation("Worker stopped successfully");
        }
    }
}