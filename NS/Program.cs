using System;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using Microsoft.SPOT.Hardware;
using System.IO;
using System.Threading;
using Common;
using Lib_Display;
using Lib_DS18B20;
using Lib_DS1307;
using System.Collections;
using MicroLiquidCrystal;
using System.Text;

//using GHI.Premium.IO;
//using GHI.Premium.Hardware;


namespace NearSpaceProj
{
  public class Program
  {
    const Cpu.Pin ledOrange = (Stm32F4Discovery.LedPins.Orange);
    const Cpu.Pin ledGreen = (Stm32F4Discovery.LedPins.Green);
    const Cpu.Pin ledBlue = (Stm32F4Discovery.LedPins.Blue);
    const Cpu.Pin ledRed = (Stm32F4Discovery.LedPins.Red);
    static OutputPort ledPortOrange;
    static OutputPort ledPortGreen;
    static OutputPort ledPortBlue;
    static OutputPort ledPortRed;

    // Hold a static reference in case the GC kicks in and disposes it automatically, note that we only support one in this example!
 
    public static void Main()
    {

      
      // Subscribe to RemovableMedia events
 
      // Create a new storage device
      //ps = new PersistentStorage("SD");
      //ps.MountFileSystem();

      // Sleep forever
 
      
      /// Inicjalizacja ponów LED
      Program.ledPortOrange = new OutputPort(ledOrange, false);
      Program.ledPortGreen = new OutputPort(ledGreen, false);
      Program.ledPortBlue = new OutputPort(ledBlue, false);
      Program.ledPortRed = new OutputPort(ledRed, false);



      //anak a dc dziala jak talala
      //AnalogInput BatteryVoltage = new AnalogInput((Cpu.AnalogChannel)Cpu.AnalogChannel.ANALOG_0);
 
     //// (; ; )
     // {
     //  double voltage = BatteryVoltage.Read();

     //  var milliVolts = (voltage * (double)3300) / (double)0xFFF;
     //  var milliVolts = voltage * 130;
     // }

      string startLog = null;


      Thread.Sleep(1000);
 
      var lcdProvider = new GpioLcdTransferProvider(Stm32F4Discovery.Pins.PD1,
                                              Stm32F4Discovery.Pins.PD2,
                                              Stm32F4Discovery.Pins.PD9,
                                              Stm32F4Discovery.Pins.PD11,
                                              Stm32F4Discovery.Pins.PD10,
                                              Stm32F4Discovery.Pins.PD8);

      var lcd = new Lcd(lcdProvider);
      lcd.Begin(16, 2);

     
      //lcd.SetCursorPosition(0, 0);
      //lcd.Write("GO!");
      //Thread.Sleep(2000);

      bool sdCardIsInserted = false;

      VolumeInfo[] volumes;

      string[] availFs = VolumeInfo.GetFileSystems();
      if (availFs.Length == 0)
      {
        //lcd.SetCursorPosition(0, 0);
        Debug.Print("No FS found");
        return;
      }

      foreach (string fs in availFs)
      {
        // lcd.SetCursorPosition(0, 0);
        Debug.Print("Av FS: " + fs);
        startLog += (fs + "\n");
        //Thread.Sleep(1000);
      }
      
      do
      {
        //VolumeInfo.GetVolumes()[0].Refresh();
        
        volumes = VolumeInfo.GetVolumes();      // List of installed volumes

        foreach (VolumeInfo volume in volumes)  // Refreach all volumes - for hot swap SD inserting
        {
          volume.Refresh();
        }

        volumes = VolumeInfo.GetVolumes();      // List of installed volumes

        if (volumes.Length == 0)
        {
          //lcd.SetCursorPosition(0, 0);
          Debug.Print("No volumes found");
          blinkAllLed();
        }
        else
        {
          int totalSize = 0;
          foreach (VolumeInfo volume in volumes)
          {
            totalSize += (int)volume.TotalSize;
          }

          if (totalSize > 0)
          {
            sdCardIsInserted = true;
          }
          else
          {
            blinkAllLed();
          }
        }

      } while (!sdCardIsInserted);


      foreach (VolumeInfo volume in volumes)
      {
        //lcd.SetCursorPosition(0, 0);
        Debug.Print("Volume: " + volume.Name);
        startLog += (volume.Name + "\n");
        Thread.Sleep(1000);
      }



      // Losowa nazwa pliku
      string ext = Program.RandomString(5);

      /*
      string[] directories = Directory.GetDirectories(@"\SD");
      lcd.Write("directory count: " + directories.Length.ToString());
      startLog += ("directory count: " + directories.Length.ToString() + "\n");

      for (int i = 0; i < directories.Length; i++)
      { 
        lcd.Write("directory: " + directories[i]);
        startLog += ("directory count: " + directories[i] + "\n");
      }
      */
      string[] files = Directory.GetFiles(@"\SD");
      startLog += ("file count: " + files.Length.ToString() + "\n");

      for (int i = 0; i < files.Length; i++)
      {
        startLog += ("filename: " + files[i] + "\n");
        //ledPortGreen.Write(true);
        //Thread.Sleep(500);
        //ledPortGreen.Write(false);
        //Thread.Sleep(500);
        Debug.Print("filename: " + files[i]);
      }

      string rtDir = VolumeInfo.GetVolumes()[0].RootDirectory;
      string path = rtDir + @"\" + (files.Length + 1) + "" + "dane" + ext + ".ns";
      /*
      FileStream fs0 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
      byte[] buffer = UTF8Encoding.UTF8.GetBytes("Start Systemu\n" + path.ToString() + "\n" + startLog.ToString() + "\n");


      if (fs0.CanRead) {
        blinkLed(ledGreen); blinkLed(ledGreen);
      } else {
        blinkLed(ledGreen);
      }

      if (fs0.CanSeek) {
        blinkLed(ledBlue); blinkLed(ledBlue);
      } else {
        blinkLed(ledBlue);
      }

      if (fs0.CanWrite) {
        blinkLed(ledRed); blinkLed(ledRed);
      } else {
        blinkLed(ledRed);
      }


      fs0.Write(buffer, 0, buffer.Length);
      fs0.Close();
      Thread.Sleep(2000); 
      */
      //startLog = null; // Odśnieżanie

      for (; ; )
      {
 
        string tx = Program.RandomString(5);

        lcd.SetCursorPosition(0, 0);
        lcd.Write((files.Length+1) + "_ZAPIS " + tx);
        
        FileStream fs1;

        if (File.Exists(path)) {
          fs1 = new FileStream(path, FileMode.Append);
          Debug.Print("Exist: " + path);
          blinkLed(ledGreen); blinkLed(ledGreen);

        } else {
          fs1 = new FileStream(path, FileMode.Create);
          Debug.Print("Not Exist: " + path);
          blinkLed(ledGreen);
        }

        byte[] bff = UTF8Encoding.UTF8.GetBytes(tx + "\n");
        /*
        if (fs1.CanRead)
        {
          blinkLed(ledGreen); blinkLed(ledGreen);
        }
        else
        {
          blinkLed(ledGreen);
        }

        if (fs1.CanSeek)
        {
          blinkLed(ledBlue); blinkLed(ledBlue);
        }
        else
        {
          blinkLed(ledBlue);
        }

        if (fs1.CanWrite)
        {
          blinkLed(ledRed); blinkLed(ledRed);
        }
        else
        {
          blinkLed(ledRed);
        }
        */
        if (startLog.Length > 0)
        {
          byte[] stbff = UTF8Encoding.UTF8.GetBytes(startLog + "\n");
          fs1.Write(stbff, 0, stbff.Length);
          startLog = "";
          stbff = null;
          blinkLed(ledRed);
        }

        fs1.Write(bff, 0, bff.Length);
        fs1.Close();
        
        
        //FileStream fs2 = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 512);
        // sr = new StreamReader(fs2);
        //string b = sr.ReadToEnd();
        //fs2.Close();
        //Thread.Sleep(50); 

        //FileStream fs3 = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None);
        //[] buffer2 = UTF8Encoding.UTF8.GetBytes("dwa" + "\n");
        //fs3.Write(buffer2, 0, buffer.Length);
        //fs3.Close();

         Thread.Sleep(500);
      }
    }

    private static void blinkAllLed()
    {
      Program.ledPortBlue.Write(true);
      Program.ledPortGreen.Write(true);
      Program.ledPortRed.Write(true);
      Program.ledPortOrange.Write(true);
      Thread.Sleep(250);
      Program.ledPortBlue.Write(false);
      Program.ledPortGreen.Write(false);
      Program.ledPortRed.Write(false);
      Program.ledPortOrange.Write(false);
      Thread.Sleep(250);
    }

    private static string RandomString(int size)
    {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < size; i++)
      {
        int it = (Program.getRandom() % 20) + 65;

        builder.Append((char)it);
      }

      return builder.ToString();
    }

    private static int getRandom()
    {
      Random random = new Random();
      int randomNumber = random.Next(100);
      return randomNumber;
    }

    private static void blinkLed(Cpu.Pin ledPin)
    {
      switch (ledPin) { 
        case Stm32F4Discovery.LedPins.Blue : 
           Program.ledPortBlue.Write(true); Thread.Sleep(250); Program.ledPortBlue.Write(false); Thread.Sleep(250);
           break;
        case Stm32F4Discovery.LedPins.Red:
           Program.ledPortRed.Write(true); Thread.Sleep(250); Program.ledPortBlue.Write(false); Thread.Sleep(250);
           break;
        case Stm32F4Discovery.LedPins.Green:
           Program.ledPortGreen.Write(true); Thread.Sleep(250); Program.ledPortGreen.Write(false); Thread.Sleep(250);
           break;
        case Stm32F4Discovery.LedPins.Orange:
           Program.ledPortOrange.Write(true); Thread.Sleep(250); Program.ledPortOrange.Write(false); Thread.Sleep(250);
           break;
      }
    }

  
  }
}
