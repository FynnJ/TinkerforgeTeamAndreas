using Tinkerforge.Worker.NotificationService;

namespace Tinkerforge.Worker.MotionSensor;

public class MotionService (ILogger<MotionService> logger, IPConnection ipConnection, ITelegramService telegramService) : IFeatureService
{
    private const string Uid = "ML4";
    private BrickletMotionDetectorV2 MotionsensorObj;
    
    public void ExecuteService()
    {
        MotionsensorObj = new BrickletMotionDetectorV2(Uid, ipConnection);
        MotionsensorObj.SetSensitivity(100);
        MotionsensorObj.MotionDetectedCallback += MotionDetected;
    }
    void MotionDetected(BrickletMotionDetectorV2 sender)
    {
        sender.SetIndicator(255,255,255);
        sender.MotionDetectedCallback -= MotionDetected;
        StartCountdown(sender);
    }

    void StartCountdown(BrickletMotionDetectorV2 sender)
    {
    const string Uid = "Tre";

        BrickletSegmentDisplay4x7V2 SegmentDisplay = new BrickletSegmentDisplay4x7V2(Uid, ipConnection);
        SegmentDisplay.StartCounter(60,0,-1,1000);
        SegmentDisplay.CounterFinishedCallback += NotAuthorizedAlarm;
        sender.SetIndicator(0,0,0);
    }

    void NotAuthorizedAlarm(BrickletSegmentDisplay4x7V2 sender)
    {
        const string Uid = "R7M";

        BrickletPiezoSpeakerV2 Alarm = new BrickletPiezoSpeakerV2(Uid, ipConnection);
        Alarm.SetAlarm(800,2000,10,1,1,10000);
        telegramService.SendMessageAsync("Unautorisierte Bewegung erkannt");
        sender.CounterFinishedCallback -= NotAuthorizedAlarm;
        MotionsensorObj.MotionDetectedCallback += MotionDetected;
    }
}