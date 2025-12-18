using MHWApp.Library.Enums;

namespace MHWApp.Client.Services.Interfaces
{
    public interface IPreferredColorSchemeService
    {
        event Action? OnChange;

        ColorScheme Scheme { get; set; }

        bool HasFetchedUsersPreferredColorScheme { get; }

        bool IsDarkMode { get; }

        Task FetchUsersPreferredColorScheme();
    }
}