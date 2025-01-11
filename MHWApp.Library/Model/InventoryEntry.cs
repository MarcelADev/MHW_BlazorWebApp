using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class InventoryEntry
    {
        public int Id { get; set; }

        public int InventoryCategoryId { get; set; }
        public InventoryEntryCategory InventoryCategory { get; set; }

        public int GameExtensionId { get; set; }
        public GameExtension GameExtension { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
