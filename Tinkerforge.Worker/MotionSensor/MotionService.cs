using Tinkerforge.Worker.Notifier;

namespace Tinkerforge.Worker.MotionSensor;

public class MotionService (ILogger<MotionService> logger, IPConnection ipConnection, ITelegramService telegramService) : IFeatureService
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
        sender.SetIndicator(255,255,255);
        StartCountdown(sender);
    }

    void StartCountdown(BrickletMotionDetectorV2 sender)
    {
    const string Uid = "Tre";

        BrickletSegmentDisplay4x7V2 SegmentDisplay = new BrickletSegmentDisplay4x7V2(Uid, ipConnection);
        SegmentDisplay.StartCounter(10,0,-1,6000);
        SegmentDisplay.CounterFinishedCallback += NotAuthorizedAlarm;
        sender.SetIndicator(0,0,0);
    }

    void NotAuthorizedAlarm(BrickletSegmentDisplay4x7V2 sender)
    {
        const string Uid = "R7M";

        BrickletPiezoSpeakerV2 Alarm = new BrickletPiezoSpeakerV2(Uid, ipConnection);
        Alarm.SetAlarm(800,2000,10,1,1,10000);
        telegramService.SendMessageAsync("Unautorisierte Bewegung erkannt");
    }
}