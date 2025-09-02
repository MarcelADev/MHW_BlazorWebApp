using static MHWApp.Client.Services.GlobalAlertService;

namespace MHWApp.Client.Services
{
    public interface IGlobalAlertService
    {
        event Action? TriggerStateHasChanged;
        IReadOnlyList<Alert> Alerts { get; }

        public void ShowSuccess(string message);
        public void ShowDanger(string message);
        public void ShowWarning(string message);
        public void ShowPrimeVu(string message);
    }
}
