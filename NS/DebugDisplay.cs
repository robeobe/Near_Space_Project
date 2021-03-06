using System;
using Microsoft.SPOT;

namespace Lib_Display
{
    public class DebugDisplay : IDisplay
    {
        public void ShowTemperature(float temperature)
        {
            Debug.Print("Temperatura: " + temperature.ToString("F2") + " �C");
        }

        public void ShowError(string message)
        {
            if (message == null) 
                throw new ArgumentNullException("message");

            Debug.Print("Error: " + message);
        }
    }
}