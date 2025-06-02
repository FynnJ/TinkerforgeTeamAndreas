namespace Tinkerforge.Worker.DataMonitoring;

using Tinkerforge;
using System.Threading;

public class DataMonitoringService(IPConnection ipConnection) : IFeatureService
{
    private readonly string _displayId = "24Rh";
    private readonly string _tempId = "Wcg";
    private readonly string _humidId = "ViW";
    private readonly string _lightId = "Pdw";
    private readonly string _buttonId = "Vd8";

    private int _currentValue = 0;

    public void ExecuteService()
    {
        var lcd = new BrickletLCD128x64(_displayId, ipConnection);
        var ptc = new BrickletPTCV2(_tempId, ipConnection);
        var humid = new BrickletHumidityV2(_humidId, ipConnection);
        var light = new BrickletAmbientLightV3(_lightId, ipConnection);
        var dualButton = new BrickletDualButtonV2(_buttonId, ipConnection);

        lcd.SetDisplayConfiguration(128, 255, false, true);
        DisplayCurrentValue(lcd, ptc, humid, light);
        
        dualButton.SetLEDState(BrickletDualButtonV2.LED_LEFT, BrickletDualButtonV2.LED_STATE_OFF);
        dualButton.SetLEDState(BrickletDualButtonV2.LED_RIGHT, BrickletDualButtonV2.LED_STATE_OFF);
        
        dualButton.StateChangedCallback += (sender, buttonL, buttonR, ledR, ledL) =>
        {
            if (buttonL == BrickletDualButtonV2.BUTTON_STATE_PRESSED)
            {
                dualButton.SetLEDState(BrickletDualButtonV2.LED_STATE_ON, BrickletDualButtonV2.LED_STATE_OFF);
                Thread.Sleep(150);
                dualButton.SetLEDState(BrickletDualButtonV2.LED_STATE_OFF, BrickletDualButtonV2.LED_STATE_OFF);

                SwitchDisplayValue(reverse: true);
                DisplayCurrentValue(lcd, ptc, humid, light);
            }

            if (buttonR == BrickletDualButtonV2.BUTTON_STATE_PRESSED)
            {
                dualButton.SetLEDState(BrickletDualButtonV2.LED_STATE_OFF, BrickletDualButtonV2.LED_STATE_ON);
                Thread.Sleep(150);
                dualButton.SetLEDState(BrickletDualButtonV2.LED_STATE_OFF, BrickletDualButtonV2.LED_STATE_OFF);

                SwitchDisplayValue(reverse: false);
                DisplayCurrentValue(lcd, ptc, humid, light);
            }
        };
    }


    private void DisplayCurrentValue(BrickletLCD128x64 lcd, BrickletPTCV2 ptc, BrickletHumidityV2 humid, BrickletAmbientLightV3 light)
    {
        var temperature = ptc.GetTemperature() / 100.0;
        var humidity = humid.GetHumidity() / 100.0;
        var lightLevel = light.GetIlluminance() / 100.0;

        lcd.ClearDisplay();

        switch (_currentValue)
        {
            case 0:
                lcd.WriteLine(0, 0, $"Temperatur: {temperature:F1} C");
                break;
            case 1:
                lcd.WriteLine(0, 0, $"Feuchtigkeit: {humidity:F1} %");
                break;
            case 2:
                lcd.WriteLine(0, 0, $"Helligkeit: {lightLevel:F1} Lux");
                break;
        }

        lcd.SetDisplayConfiguration(128, 255, false, true);
    }

    private void SwitchDisplayValue(bool reverse)
    {
        if (reverse)
        {
            _currentValue = (_currentValue == 0) ? 2 : _currentValue - 1;
        }
        else
        {
            _currentValue = (_currentValue + 1) % 3;
        }
    }
}
