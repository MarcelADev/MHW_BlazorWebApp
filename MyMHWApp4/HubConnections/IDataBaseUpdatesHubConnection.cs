using Microsoft.AspNetCore.SignalR.Client;

namespace MyMHWApp4.HubConnections
{
    public interface IDataBaseUpdatesHubConnection
    {
        HubConnection Connection { get; }
    }
}
