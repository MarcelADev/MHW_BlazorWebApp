using MHWApp.Library.Model;
using Microsoft.AspNetCore.SignalR.Client;

namespace MyMHWApp4.HubConnections
{
    public interface IDataBaseUpdatesHubConnection
    {
        HubConnection Connection { get; }

        event Action<GameExtension>? OnNewGameExtension;
        event Action<GameExtension>? OnUpdatedGameExtension;
        event Action<int>? OnDeletedGameExtension;

        event Action<Category>? OnNewCategory;
        event Action<Category>? OnUpdatedCategory;
        event Action<int>? OnDeletedCategory;
    }
}
