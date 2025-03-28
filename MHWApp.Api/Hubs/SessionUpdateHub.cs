using Microsoft.AspNetCore.SignalR;

namespace MHWApp.Api.Hubs
{
    public class SessionUpdateHub : Hub
    {
        public async Task NotifyClientsToFetchSessions()
        {
            await Clients.All.SendAsync("ReceiveSessionUpdateNotification");
        }
    }
}
