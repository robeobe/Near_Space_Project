namespace Lib_Display
{
    public interface IDisplay
    {
        void ShowTemperature(float temperature);
        void ShowError(string message);
    }
}