using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class ArmorCraftingIngredient
    {
        public int Id { get; set; }

        public int ArmorId { get; set; }
        public required Armor Armor { get; set; }

        public int InventoryEntryId { get; set; }
        public required InventoryEntry InventoryEntry { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
