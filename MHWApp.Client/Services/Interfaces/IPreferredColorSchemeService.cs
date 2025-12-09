using MHWApp.Library.Enums;

namespace MHWApp.Client.Services.Interfaces
{
    public interface IPreferredColorSchemeService
    {
        event Action? OnChange;

        ColorScheme Scheme { get; set; }

        bool IsInitialized { get; }

        bool IsDarkMode { get; }
    }
}