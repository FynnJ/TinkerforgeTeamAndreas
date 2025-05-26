namespace Tinkerforge.Worker.DataMonitoring;

public class DataMonitoringWorker(
    ILogger<Worker> logger,
    DataMonitoringService dataMonitoringService)
    : TinkerforgeWorkerBase(logger)
{
    protected override void ExecuteService()
    {
        dataMonitoringService.ExecuteService();
    }
}