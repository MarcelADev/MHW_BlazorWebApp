using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace MHWApp.Client.Services
{
    public class SessionUpdateService
    {
        private readonly HubConnection _hubConnection;

        public SessionUpdateService(NavigationManager navigationManager)
        {
            // Create the SignalR Hub connection
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/dataUpdateHub"))
                .Build();
        }

        // Start the connection and handle the notifications
        public async Task StartAsync()
        {
            _hubConnection.On("ReceiveSessionUpdateNotification", async () =>
            {
                // This method will be called when the server broadcasts the notification
                await HandleDataUpdate();
            });

            await _hubConnection.StartAsync();
        }

        private async Task HandleDataUpdate()
        {
            // Here you can trigger logic to fetch the latest data from your API
            // For example, make an HTTP request to your backend to fetch the updated data
            Console.WriteLine("Data update received. Fetching new data...");
            // Call your data fetching logic here
        }

        // Stop the connection when the component is disposed or no longer needed
        public async Task StopAsync()
        {
            await _hubConnection.StopAsync();
        }
    }
}
