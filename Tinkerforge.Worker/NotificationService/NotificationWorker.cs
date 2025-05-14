namespace Tinkerforge.Worker.NotificationService;

public class NotificationWorker(
    ILogger<Worker> logger,
    NotificationService notificationService)
    : TinkerforgeWorkerBase(logger)
{
    protected override void ExecuteService()
    {
        notificationService.ExecuteService();
    }
}