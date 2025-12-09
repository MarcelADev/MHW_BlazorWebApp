namespace MHWApp.Client.Services.Interfaces
{
    public interface IScreenSizeService
    {
        event Action? OnChange;
        int Width { get; set; }
        bool ScreenIsWide { get; }
    }
}