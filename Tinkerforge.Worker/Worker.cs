using Tinkerforge.Worker.Notifier;
using Tinkerforge.Worker.MotionSensor;

namespace Tinkerforge.Worker;

public class Worker(
    ILogger<Worker> logger,
    IPConnection ipconnection,
    NotifierService notifierService,
    MotionService motionService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            motionService.ExecuteService();
            notifierService.ExecuteService();

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
        }
        finally
        {
            ipconnection.Disconnect();
            logger.LogInformation("Successfully disconnected from Tinkerforge");
        }
    }
}