using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class WeaponCraftingIngredient
    {
        public int Id { get; set; }

        public int WeaponId { get; set; }
        public required Weapon Weapon { get; set; }

        public int InventoryEntryId { get; set; }
        public required InventoryEntry InventoryEntry { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
