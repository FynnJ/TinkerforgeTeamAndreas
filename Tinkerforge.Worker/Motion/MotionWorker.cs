namespace Tinkerforge.Worker.Motion;

public class MotionWorker(
    ILogger<Worker> logger,
    MotionService motionService)
    : TinkerforgeWorkerBase(logger)
{
    protected override void ExecuteService()
    {
        motionService.ExecuteService();
    }
}