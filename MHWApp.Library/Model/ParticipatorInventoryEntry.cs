namespace MHWApp.Library.Model
{
    public class ParticipatorInventoryEntry
    {
        public int Id { get; set; }

        public int ParticipatorId { get; set; }
        public Participator Participator { get; set; }

        public int InventoryEntryId { get; set; }
        public InventoryEntry InventoryEntry { get; set; }

        public int Count { get; set; }
    }
}
