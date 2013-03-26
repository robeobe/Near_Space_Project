using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO;
using System.Threading;
using Common;
using Bib_Display;
using Bib_DS18B20;
using System.Collections;

namespace NS_ONE_WIRE
{
  public class Program
  {
    public static void Main()
    {
      var op = new OutputPort(Stm32F4Discovery.FreePins.PA15, false);
      var ow = new OneWire(op);

      IDisplay display = new Display();
      DS18X20[] devices = DS18X20.FindAll(ow);
      if (devices.Length == 0)
      {
        display.ShowError("Brak DS18B20");
        return;
      }
      else {
        ArrayList dev = ow.FindAllDevices();
        
        Debug.Print("Found " + dev.Count);

        display.ShowTemperature(devices.Length);
      }
      
      DS18X20 tempDev = devices[0];
      for (; ; )
      {
        float currentTemp = 0;
        string exceptionMsg = String.Empty;
        try
        {
          currentTemp = tempDev.GetTemperature();
        }
        catch (IOException ex)
        {
          exceptionMsg = ex.Message;
        }

        if (exceptionMsg.Length == 0)
          display.ShowTemperature(currentTemp);
        else
          display.ShowError(exceptionMsg);

        Thread.Sleep(1000);
      }
    }

  }
}
