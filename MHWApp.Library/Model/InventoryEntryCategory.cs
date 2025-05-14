using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class InventoryEntryCategory : ModelBase<int>
    {
        public override required int Id { get; set; }

        public required int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public required int Order { get; set; }

        public List<InventoryEntry> InventoryEntries { get; set; } = [];
    }
}
