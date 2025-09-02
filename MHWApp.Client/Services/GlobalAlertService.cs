namespace MHWApp.Client.Services
{
    public class GlobalAlertService : IGlobalAlertService
    {
        public event Action? TriggerStateHasChanged;

        public record Alert(string Message, AlertType Type);

        public enum AlertType
        {
            Success,
            Danger,
            Warning,
            PrimeVu
        }

        private readonly List<Alert> _alerts = new();
        public IReadOnlyList<Alert> Alerts => _alerts.AsReadOnly();

        public void ShowSuccess(string message) => _ = AddAlert(message, AlertType.Success);
        public void ShowDanger(string message) => _ = AddAlert(message, AlertType.Danger);
        public void ShowWarning(string message) => _ = AddAlert(message, AlertType.Warning);
        public void ShowPrimeVu(string message) => _ = AddAlert(message, AlertType.PrimeVu);

        private async Task AddAlert(string message, AlertType type)
        {
            Alert newAlert = new(message, type);
            _alerts.Add(newAlert);
            TriggerStateHasChanged?.Invoke();

            //await Task.Delay(8000);

            //_alerts.Remove(newAlert);
            //TriggerStateHasChanged?.Invoke();
        }
    }
}
