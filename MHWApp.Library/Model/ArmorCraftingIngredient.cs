using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class ArmorCraftingIngredient
    {
        public required int Id { get; set; }

        public required int ArmorId { get; set; }
        public Armor? Armor { get; set; }

        public required int InventoryEntryId { get; set; }
        public InventoryEntry? InventoryEntry { get; set; }

        [Required]
        public required int Quantity { get; set; }
    }
}
