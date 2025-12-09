namespace MHWApp.Client.Services.Interfaces
{
    public interface IScreenSizeService
    {
        public event Action? OnChange;
        public int Width { get; set; }
        public bool ScreenIsWide => Width >= 768;
    }
}