using MHWApp.Library.Model;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyMHWApp4.HubConnections
{
    public class DataBaseUpdatesHubConnection : IDataBaseUpdatesHubConnection
    {
        public HubConnection Connection { get; }

        public event Action<GameExtension>? OnNewGameExtension;
        public event Action<GameExtension>? OnUpdatedGameExtension;
        public event Action<int>? OnDeletedGameExtension;

        public DataBaseUpdatesHubConnection(IConfiguration configuration)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(configuration["BackendUrl"] + "/databaseupdateshub")
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();

            _ = EnsureConnectAsync();
        }

        private void RegisterHandlers()
        {
            Connection.On<GameExtension>("ReceiveNewGameExtension", (data) =>
            {
                OnNewGameExtension?.Invoke(data);
            });
            Connection.On<GameExtension>("ReceiveUpdatedGameExtension", (data) =>
            {
                OnUpdatedGameExtension?.Invoke(data);
            });

            Connection.On<int>("ReceiveDeletedGameExtension", (deletedId) =>
            {
                OnDeletedGameExtension?.Invoke(deletedId);
            });

            //Extendable
        }

        private async Task EnsureConnectAsync()
        {
            if (Connection.State == HubConnectionState.Disconnected) await Connection.StartAsync();
        }
    }
}
