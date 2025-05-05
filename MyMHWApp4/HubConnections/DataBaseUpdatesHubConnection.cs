using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyMHWApp4.HubConnections
{
    public class DataBaseUpdatesHubConnection : IDataBaseUpdatesHubConnection
    {
        public HubConnection Connection { get; }
        public DataBaseUpdatesHubConnection(IConfiguration configuration)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(configuration["BackendUrl"] + "/databaseupdateshub")
                .WithAutomaticReconnect()
                .Build();
        }
    }
}

// ein Listener Service erstellen und diesen an relevanten stellen injecten
