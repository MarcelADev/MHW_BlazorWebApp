namespace MHWApp.Library.Model
{
    public class ParticipatorInventoryEntry
    {
        public required int Id { get; set; }

        public required int ParticipatorId { get; set; }
        public Participator? Participator { get; set; }

        public required int InventoryEntryId { get; set; }
        public InventoryEntry? InventoryEntry { get; set; }

        public required int Count { get; set; }
    }
}
