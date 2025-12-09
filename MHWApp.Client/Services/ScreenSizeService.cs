using MHWApp.Client.Services.Interfaces;

namespace MHWApp.Client.Services
{
    public class ScreenSizeService : IScreenSizeService
    {
        public event Action? OnChange;

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnChange?.Invoke();
                }
            }
        }

        public bool ScreenIsWide => Width >= 768;
    }
}