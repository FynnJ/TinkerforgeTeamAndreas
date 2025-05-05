using Tinkerforge.Worker.DataMonitoring;
using Tinkerforge.Worker.Notifier;

namespace Tinkerforge.Worker;

public class Worker(
    ILogger<Worker> logger,
    IPConnection ipconnection,
    NotifierService notifierService,
    DataMonitoringService dataMonitoringService
    )
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                notifierService.ExecuteService();
                dataMonitoringService.ExecuteService();

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