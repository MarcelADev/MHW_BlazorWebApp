using MHWApp.Library.Model;
using Microsoft.AspNetCore.SignalR.Client;

namespace MyMHWApp4.HubConnections
{
    public interface IDataBaseUpdatesHubConnection
    {
        HubConnection Connection { get; }

        // ## SESSION ##
        event Action<Session>? OnNewSession;
        event Action<Session>? OnUpdatedSession;
        event Action<int>? OnDeletedSession;

        // ## SESSION ##
        event Action<int>? OnDeletedParticipator;

        // ## GAME EXTENSION ##
        event Action<GameExtension>? OnNewGameExtension;
        event Action<GameExtension>? OnUpdatedGameExtension;
        event Action<int>? OnDeletedGameExtension;

        // ## CATEGORY & SUB-CATEGORIES ##
        event Action<Category>? OnNewCategory;
        event Action<Category>? OnUpdatedCategory;
        event Action<int>? OnDeletedCategory;

        event Action<InventoryEntryCategory>? OnNewInventoryCategory;
        event Action<int>? OnDeletedInventoryCategory;

        event Action<WeaponCategory>? OnNewWeaponCategory;
        event Action<int>? OnDeletedWeaponCategory;

        event Action<ArmorCategory>? OnNewArmorCategory;
        event Action<int>? OnDeletedArmorCategory;

        // ## INVENTORY ENTRY ##
        event Action<InventoryEntry>? OnNewInventoryEntry;
        event Action<InventoryEntry>? OnUpdatedInventoryEntry;
        event Action<int>? OnDeletedInventoryEntry;
    }
}
