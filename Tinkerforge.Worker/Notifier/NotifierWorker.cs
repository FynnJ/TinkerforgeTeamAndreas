namespace Tinkerforge.Worker.Notifier;

public class NotifierWorker(
    ILogger<Worker> logger,
    NotifierService notifierService)
    : TinkerforgeWorkerBase(logger)
{
    protected override void ExecuteService()
    {
        notifierService.ExecuteService();
    }
}