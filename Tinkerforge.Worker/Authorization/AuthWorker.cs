namespace Tinkerforge.Worker.Authorization;

public class AuthWorker(ILogger<Worker> logger, AuthService service) : TinkerforgeWorkerBase(logger)
{
    protected override void ExecuteService()
    {
        service.Initialize();
    }
}