using static MHWApp.Client.Services.GlobalAlertService;

namespace MHWApp.Client.Services.Interfaces
{
    public interface IGlobalAlertService
    {
        event Action? TriggerStateHasChanged;
        IReadOnlyList<Alert> Alerts { get; }

        void ShowSuccess(string message);
        void ShowDanger(string message);
        void ShowWarning(string message);
        void ShowInfo(string message);
        void ShowPrimeVu(string message);

        void ShowInfoWithoutTimer(string message);
        void RemoveInfoManually(string message);
    }
}
