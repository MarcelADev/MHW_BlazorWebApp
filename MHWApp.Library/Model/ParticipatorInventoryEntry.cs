namespace MHWApp.Library.Model
{
    public class ParticipatorInventoryEntry : IModel
    {
        public int Id { get; set; }

        public int ParticipatorId { get; set; }
        public required Participator Participator { get; set; }

        public int InventoryEntryId { get; set; }
        public required InventoryEntry InventoryEntry { get; set; }

        public int Count { get; set; }
    }
}
