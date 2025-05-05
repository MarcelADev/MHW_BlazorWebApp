using MHWApp.Library.Model;
using Microsoft.AspNetCore.SignalR;

namespace MHWApp.Api.Hubs
{
    public class DataBaseUpdatesHub : Hub
    {
        public async Task NotifyNewGameExtension(GameExtension newGameExtension)
        {
            await Clients.All.SendAsync("ReceiveNewGameExtension", newGameExtension);
        }
    }
}
