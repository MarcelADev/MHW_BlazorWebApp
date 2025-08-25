using Microsoft.AspNetCore.SignalR.Client;

namespace MHWApp.Client.HubConnections
{
    public interface IChatHubConnection
    {
        HubConnection Connection { get; }
    }
}
