using Microsoft.AspNetCore.SignalR.Client;

namespace MyMHWApp4.HubConnections
{
    public interface IChatHubConnection
    {
        HubConnection Connection { get; }
    }
}
