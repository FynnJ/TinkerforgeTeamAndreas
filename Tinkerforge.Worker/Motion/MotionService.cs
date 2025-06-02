using Tinkerforge.Worker.NotificationService;
// using Tinkerforge.Worker.Authorization;
using Microsoft.Extensions.Primitives;

namespace Tinkerforge.Worker.Motion;

public class MotionService(ILogger<MotionService> logger, IPConnection ipConnection, ITelegramService telegramService) : IFeatureService
{
    private const string Uid = "ML4";
    private const string SegmentUid = "Tre";
    private BrickletNFC                 nfc = new BrickletNFC(nfcUid, ipConnection);
    private BrickletEPaper296x128             eInk = new BrickletEPaper296x128(eInkUid, ipConnection);
    private BrickletRGBLEDButton             ledButton = new BrickletRGBLEDButton(rgbUid, ipConnection);
    private static readonly string nfcUid = "22ND";
    private static readonly string eInkUid = "24KJ";
    private static readonly string rgbUid = "23Qx";

        private readonly HashSet<string> _authorizedUids = new()
    {
        "04DA7B7AFE1D90",
        "04EC6642B91190",
        "04E3F4B78F6180"
    };

    private BrickletMotionDetectorV2 MotionsensorObj;
    private BrickletSegmentDisplay4x7V2 SegmentDisplay = new BrickletSegmentDisplay4x7V2(SegmentUid, ipConnection);

    public void ExecuteService()
    {
        MotionsensorObj = new BrickletMotionDetectorV2(Uid, ipConnection);
        MotionsensorObj.SetSensitivity(50);
        MotionsensorObj.MotionDetectedCallback += MotionDetected;
    }
    void MotionDetected(BrickletMotionDetectorV2 sender)
    {
        sender.SetIndicator(255, 255, 255);
        sender.MotionDetectedCallback -= MotionDetected;
        StartCountdown(sender);
    }

    void StartCountdown(BrickletMotionDetectorV2 sender)
    {

        SegmentDisplay.StartCounter(60, 0, -1, 1000);
        SegmentDisplay.CounterFinishedCallback += NotAuthorizedAlarm;

        RequestAuthorization();

        sender.SetIndicator(0, 0, 0);
    }

    void NotAuthorizedAlarm(BrickletSegmentDisplay4x7V2 sender)
    {
        const string Uid = "R7M";

        BrickletPiezoSpeakerV2 Alarm = new BrickletPiezoSpeakerV2(Uid, ipConnection);
        Alarm.SetAlarm(800, 2000, 10, 1, 1, 10000);
        telegramService.SendMessageAsync("Unautorisierte Bewegung erkannt");
        sender.CounterFinishedCallback -= NotAuthorizedAlarm;
        MotionsensorObj.MotionDetectedCallback += MotionDetected;
    }

    public void StopAlarm()
    {
        SegmentDisplay.CounterFinishedCallback -= NotAuthorizedAlarm;
        SegmentDisplay.SetNumericValue([0, 0, 0, 0]);
        MotionsensorObj.MotionDetectedCallback += MotionDetected;
    }
    
     public void RequestAuthorization()
    {
        try
        {
            eInk.FillDisplay(BrickletEPaper296x128.COLOR_BLACK);
            nfc.ReaderStateChangedCallback += OnReaderStateChanged;
            ledButton.ButtonStateChangedCallback += OnButtonStateChanged;

            ResetAuthenticationState();

            nfc.SetMode(BrickletNFC.MODE_READER);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Initialisierung fehlgeschlagen: {ex.Message}");
        }
    }

    private void OnReaderStateChanged(BrickletNFC sender, byte state, bool idle)
    {
        if (state == BrickletNFC.READER_STATE_REQUEST_TAG_ID_READY)
        {
            try
            {
                // UID holen
                sender.ReaderGetTagID(out byte tagType, out byte[] tagId);
                string uid = BitConverter.ToString(tagId).Replace("-", "");

                Console.WriteLine($"Karte erkannt: {uid}");

                if (_authorizedUids.Contains(uid))
                {
                    Console.WriteLine($"Zugriff gestattet: {uid}");
                    eInk.DrawText(16, 48, BrickletEPaper296x128.FONT_24X32,
                        BrickletEPaper296x128.COLOR_WHITE,
                        BrickletEPaper296x128.ORIENTATION_HORIZONTAL, "Hello User");
                    eInk.Draw();
                    StopAlarm();
                    ledButton.SetColor(0, 255, 0);
                    // Timeout für Session-Ende setzen
                }
                else
                {
                    Console.WriteLine($"Zugriff verweigert: {uid}");
                    eInk.DrawText(16, 48, BrickletEPaper296x128.FONT_24X32,
                        BrickletEPaper296x128.COLOR_WHITE,
                        BrickletEPaper296x128.ORIENTATION_HORIZONTAL, "Access Denied");
                    eInk.Draw();
                    ledButton.SetColor(255, 0, 0);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Auslesen der UID: {ex.Message}");
            }
        }
        if(idle)
        {
            sender.ReaderRequestTagID();
        }
    }
    private void OnButtonStateChanged(BrickletRGBLEDButton sender, byte state)
    {
        if (state == 0)
        {
            ResetAuthenticationState();
        }
    }
       private void ResetAuthenticationState()
    {
        eInk.FillDisplay(BrickletEPaper296x128.COLOR_BLACK);
        eInk.DrawText(16, 48, BrickletEPaper296x128.FONT_24X32, 
            BrickletEPaper296x128.COLOR_WHITE,
            BrickletEPaper296x128.ORIENTATION_HORIZONTAL, "Ready");
        eInk.Draw();
    
        ledButton.SetColor(0, 0, 255); // Blau für Bereitschaft
    }

}
