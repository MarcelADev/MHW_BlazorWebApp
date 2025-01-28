using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class WeaponCraftingIngredient
    {
        public required int Id { get; set; }

        public required int WeaponId { get; set; }
        public Weapon? Weapon { get; set; }

        public required int InventoryEntryId { get; set; }
        public InventoryEntry? InventoryEntry { get; set; }

        [Required]
        public required int Quantity { get; set; }
    }
}
