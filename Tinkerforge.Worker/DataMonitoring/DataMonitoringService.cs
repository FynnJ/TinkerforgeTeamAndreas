namespace Tinkerforge.Worker.DataMonitoring;

using Microsoft.Extensions.Logging;
using Tinkerforge;
using System.Threading;

public class DataMonitoringService(IPConnection ipConnection) : IFeatureService
{
    private static string DisplayUID = "24Rh";
    private static string TempUID = "Wcg";
    private static string HumidUID = "ViW";
    private static string LightUID = "Pdw";
    private static string ButtonUID = "Vg8";

    private int currentValue = 0;

    public void ExecuteService()
    {
        var lcd = new BrickletLCD128x64(DisplayUID, ipConnection);
        var ptc = new BrickletPTCV2(TempUID, ipConnection);
        var humid = new BrickletHumidityV2(HumidUID, ipConnection);
        var light = new BrickletAmbientLightV3(LightUID, ipConnection);
        var dualButton = new BrickletDualButtonV2(ButtonUID, ipConnection);

        // LCD Setup
        lcd.SetDisplayConfiguration(128, 255, false, true);

        DisplayCurrentValue(lcd, ptc, humid, light);

        while (true)
        {
            byte buttonL, buttonR;
            dualButton.GetButtonState(out buttonL, out buttonR);

            if (buttonL == 0) 
            {
                Thread.Sleep(20);
                SwitchDisplayValue(reverse: true); 
                DisplayCurrentValue(lcd, ptc, humid, light);
            }
            else if (buttonR == 0)
            {
                Thread.Sleep(20);
                SwitchDisplayValue(reverse: false);
                DisplayCurrentValue(lcd, ptc, humid, light);
            }

            Thread.Sleep(1);
        }
    }

    private void DisplayCurrentValue(BrickletLCD128x64 lcd, BrickletPTCV2 ptc, BrickletHumidityV2 humid, BrickletAmbientLightV3 light)
    {
        double temperature = ptc.GetTemperature() / 100.0;
        double humidity = humid.GetHumidity() / 100.0;
        double lightLevel = light.GetIlluminance() / 100.0;

        lcd.ClearDisplay();

        switch (currentValue)
        {
            case 0:
                lcd.WriteLine(0, 0, $"Temp: {temperature:F1} °C");
                break;
            case 1:
                lcd.WriteLine(0, 0, $"Humidity: {humidity:F1} %");
                break;
            case 2:
                lcd.WriteLine(0, 0, $"Light: {lightLevel:F1} Lux");
                break;
        }

        lcd.SetDisplayConfiguration(128, 255, false, true);
    }

    private void SwitchDisplayValue(bool reverse)
    {
        if (reverse)
        {
            currentValue = (currentValue == 0) ? 2 : currentValue - 1;
        }
        else
        {
            currentValue = (currentValue + 1) % 3;
        }
    }
}
