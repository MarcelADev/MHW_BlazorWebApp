using Microsoft.AspNetCore.SignalR.Client;

namespace MHWApp.Client.HubConnections
{
    public class ChatHubConnection : IChatHubConnection
    {
        public HubConnection Connection { get; }

        public ChatHubConnection(IConfiguration configuration)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(configuration["BackendUrl"] + "/chathub")
                .WithAutomaticReconnect()
                .Build();
        }
    }
}
