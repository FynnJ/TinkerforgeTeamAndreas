namespace Tinkerforge.Worker.MotionSensor;

public class MotionService (ILogger<MotionService> logger, IPConnection ipConnection) : IFeatureService
{
    private const string Uid = "ML4";
    public void ExecuteService()
    {
        BrickletMotionDetectorV2 MotionSensor = new BrickletMotionDetectorV2(Uid, ipConnection);
        MotionSensor.SetSensitivity(100);
        MotionSensor.MotionDetectedCallback += MotionDetected;
    }
    void MotionDetected(BrickletMotionDetectorV2 sender)
    {
        sender.SetIndicator(125,255,255);
        Console.WriteLine("Motion Detected", sender.GetMotionDetected());
        
        StartCountdown(sender);
    }

    void StartCountdown(BrickletMotionDetectorV2 sender)
    {
    const string Uid = "Tre";

        BrickletSegmentDisplay4x7V2 SegmentDisplay = new BrickletSegmentDisplay4x7V2(Uid, ipConnection);
        SegmentDisplay.StartCounter(10,0,-1,1000);
        SegmentDisplay.CounterFinishedCallback += NotAuthorizedAlarm;
    }

    void NotAuthorizedAlarm(BrickletSegmentDisplay4x7V2 sender)
    {
        const string Uid = "R7M";

        BrickletPiezoSpeakerV2 Alarm = new BrickletPiezoSpeakerV2(Uid, ipConnection);
        Alarm.SetAlarm(800,2000,10,1,1,10000);
    }
}