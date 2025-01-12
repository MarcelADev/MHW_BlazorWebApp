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

        [Required]
        public int Order { get; set; }

        public List<Participator> Participators { get; set; } = [];
        public List<ParticipatorInventoryEntry> ParticipatorInventoryEntries { get; set; } = [];

        public List<Weapon> Weapons { get; set; }
        public List<WeaponCraftingIngredient> WeaponCraftingIngredients { get; set; }

        public List<Armor> Armors { get; set; }
        public List<ArmorCraftingIngredient> ArmorCraftingIngredients { get; set; }
    }
}
