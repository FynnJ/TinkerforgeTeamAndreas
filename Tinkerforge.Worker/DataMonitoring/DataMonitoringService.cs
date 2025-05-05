namespace Tinkerforge.Worker.DataMonitoring;

using System;
using Microsoft.Extensions.Logging;
using Tinkerforge;

public class DataMonitoringService(ILogger<DataMonitoringService> logger, IPConnection ipConnection) : IFeatureService
{
    private static string DisplayUID = "Tre";
    private static string TempUID = "Wcg";

    public void ExecuteService()
    {
        var display = new BrickletSegmentDisplay4x7V2(DisplayUID, ipConnection);
        display.SetBrightness(7);

        var ptc = new BrickletPTCV2(TempUID, ipConnection);
        int tempRaw = ptc.GetTemperature(); 
        double temperature = tempRaw / 100.0;

        int roundedTemp = (int)Math.Round(temperature);
        short[] values = ConvertToNumericArray(roundedTemp);
        display.SetNumericValue(values);

        logger.LogInformation($"Aktuelle Temperatur: {temperature} °C");
    }

    private short[] ConvertToNumericArray(int value)
    {
        string strValue = value.ToString().PadLeft(4);
        short[] result = new short[4];

        for (int i = 0; i < 4; i++)
        {
            result[i] = strValue[i] == ' ' ? (short)-1 : short.Parse(strValue[i].ToString());
        }

        return result;
    }
}
