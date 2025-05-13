using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class InventoryEntryAndCategory
    {
        public required int Id { get; set; }

        public required int InventoryEntryId { get; set; }
        public InventoryEntry? InventoryEntry { get; set; }

        public required int InventoryCategoryId { get; set; }
        public InventoryEntryCategory? InventoryCategory { get; set; }
    }
}
