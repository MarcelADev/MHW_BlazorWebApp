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
            Info,
            PrimeVu
        }

        private readonly List<Alert> _alerts = new();
        public IReadOnlyList<Alert> Alerts => _alerts.AsReadOnly();

        public void ShowSuccess(string message) => _ = AddAlertAsync(message, AlertType.Success);
        public void ShowDanger(string message) => _ = AddAlertAsync(message, AlertType.Danger);
        public void ShowWarning(string message) => _ = AddAlertAsync(message, AlertType.Warning);
        public void ShowInfo(string message) => _ = AddAlertAsync(message, AlertType.Info);
        public void ShowPrimeVu(string message) => _ = AddAlertAsync(message, AlertType.PrimeVu);

        public void ShowInfoWithoutTimer(string message) => AddAlertWithoutTimer(message, AlertType.Info);
        public void RemoveInfoManually(string message) => RemoveAlertManually(message, AlertType.Info);

        private async Task AddAlertAsync(string message, AlertType type)
        {
            Alert newAlert = new(message, type);
            _alerts.Add(newAlert);
            TriggerStateHasChanged?.Invoke();

            await Task.Delay(8000);

            _alerts.Remove(newAlert);
            TriggerStateHasChanged?.Invoke();
        }

        private void AddAlertWithoutTimer(string message, AlertType type)
        {
            Alert newAlert = new(message, type);
            _alerts.Add(newAlert);
            TriggerStateHasChanged?.Invoke();
        }

        private void RemoveAlertManually(string message, AlertType type)
        {
            var alert = _alerts.First(a => a.Message.Equals(message) && a.Type == type);
            _alerts.Remove(alert);
            TriggerStateHasChanged?.Invoke();
        }
    }
}
