using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Category
    {
        public required int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required int GameExtensionId { get; set; }
        public GameExtension? GameExtension { get; set; }

        public InventoryEntryCategory? InventoryEntryCategory { get; set; }
        public WeaponCategory? WeaponCategory { get; set; }
        public ArmorCategory? ArmorCategory { get; set; }
    }
}
