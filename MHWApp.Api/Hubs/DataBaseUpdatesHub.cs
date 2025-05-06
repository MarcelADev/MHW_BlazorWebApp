using MHWApp.Library.Model;
using Microsoft.AspNetCore.SignalR;

namespace MHWApp.Api.Hubs
{
    public class DataBaseUpdatesHub : Hub
    {
        // Methoden im Hub sind dafür da, Server Methoden vom Client aus anzustoßen/invoken

        //public async Task NotifyNewGameExtension(GameExtension newGameExtension)
        //{
        //    await Clients.All.SendAsync("ReceiveNewGameExtension", newGameExtension);
        //}
    }
}
