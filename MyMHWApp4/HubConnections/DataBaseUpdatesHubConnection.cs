using MHWApp.Library.Model;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyMHWApp4.HubConnections
{
    public class DataBaseUpdatesHubConnection : IDataBaseUpdatesHubConnection
    {
        public HubConnection Connection { get; }

        public event Action<Session>? OnNewSession;
        public event Action<Session>? OnUpdatedSession;
        public event Action<int>? OnDeletedSession;

        public event Action<int>? OnDeletedParticipator;

        public event Action<GameExtension>? OnNewGameExtension;
        public event Action<GameExtension>? OnUpdatedGameExtension;
        public event Action<int>? OnDeletedGameExtension;

        public event Action<Category>? OnNewCategory;
        public event Action<Category>? OnUpdatedCategory;
        public event Action<int>? OnDeletedCategory;

        public event Action<InventoryEntryCategory>? OnNewInventoryCategory;
        public event Action<int>? OnDeletedInventoryCategory;

        public event Action<WeaponCategory>? OnNewWeaponCategory;
        public event Action<int>? OnDeletedWeaponCategory;

        public event Action<ArmorCategory>? OnNewArmorCategory;
        public event Action<int>? OnDeletedArmorCategory;

        public event Action<InventoryEntry>? OnNewInventoryEntry;
        public event Action<InventoryEntry>? OnUpdatedInventoryEntry;
        public event Action<int>? OnDeletedInventoryEntry;

        public DataBaseUpdatesHubConnection(IConfiguration configuration)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(configuration["BackendUrl"] + "/databaseupdateshub")
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();

            _ = EnsureConnectAsync();
        }

        private void RegisterHandlers()
        {
            // ## Session ##
            Connection.On<Session>("ReceiveNewSession", (data) =>
            {
                OnNewSession?.Invoke(data);
            });
            Connection.On<Session>("ReceiveUpdatedSession", (data) =>
            {
                OnUpdatedSession?.Invoke(data);
            });

            Connection.On<int>("ReceiveDeletedSession", (deletedId) =>
            {
                OnDeletedSession?.Invoke(deletedId);
            });

            // ## Participator ##
            Connection.On<int>("ReceiveDeletedParticipator", (sessionIdOfDeletedParticipator) =>
            {
                OnDeletedSession?.Invoke(sessionIdOfDeletedParticipator);
            });

            // ## GAME EXTENSION ##
            Connection.On<GameExtension>("ReceiveNewGameExtension", (data) =>
            {
                OnNewGameExtension?.Invoke(data);
            });
            Connection.On<GameExtension>("ReceiveUpdatedGameExtension", (data) =>
            {
                OnUpdatedGameExtension?.Invoke(data);
            });

            Connection.On<int>("ReceiveDeletedGameExtension", (deletedId) =>
            {
                OnDeletedGameExtension?.Invoke(deletedId);
            });

            // ## Category ##
            Connection.On<Category>("ReceiveNewCategory", (data) =>
            {
                OnNewCategory?.Invoke(data);
            });

            Connection.On<Category>("ReceiveUpdatedCategory", (data) =>
            {
                OnUpdatedCategory?.Invoke(data);
            });

            Connection.On<int>("ReceiveDeletedCategory", (deletedId) =>
            {
                OnDeletedCategory?.Invoke(deletedId);
            });

            // ## Inventory Category ##
            Connection.On<InventoryEntryCategory>("ReceiveNewInventoryCategory", (data) =>
            {
                OnNewInventoryCategory?.Invoke(data);
            });

            Connection.On<int>("ReceiveDeletedInventoryCategory", (deletedId) =>
            {
                OnDeletedInventoryCategory?.Invoke(deletedId);
            });

            // ## Weapon Category ##
            Connection.On<WeaponCategory>("ReceiveNewWeaponCategory", (data) =>
            {
                OnNewWeaponCategory?.Invoke(data);
            });

            Connection.On<int>("ReceiveDeletedWeaponCategory", (deletedId) =>
            {
                OnDeletedWeaponCategory?.Invoke(deletedId);
            });

            // ## Armor Category ##
            Connection.On<ArmorCategory>("ReceiveNewArmorCategory", (data) =>
            {
                OnNewArmorCategory?.Invoke(data);
            });

            Connection.On<int>("ReceiveDeletedArmorCategory", (deletedId) =>
            {
                OnDeletedArmorCategory?.Invoke(deletedId);
            });

            // ## Inventory Entry ##
            Connection.On<InventoryEntry>("ReceiveNewInventoryEntry", (data) =>
            {
                OnNewInventoryEntry?.Invoke(data);
            });

            Connection.On<InventoryEntry>("ReceiveUpdatedInventoryEntry", (data) =>
            {
                OnUpdatedInventoryEntry?.Invoke(data);
            });

            Connection.On<int>("ReceiveDeletedInventoryEntry", (deletedId) =>
            {
                OnDeletedInventoryEntry?.Invoke(deletedId);
            });
        }

        private async Task EnsureConnectAsync()
        {
            if (Connection.State == HubConnectionState.Disconnected) await Connection.StartAsync();
        }
    }
}
