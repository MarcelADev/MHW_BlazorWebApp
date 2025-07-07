using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class InventoryEntry
    {
        public required int Id { get; set; }

        [Required]
        public required int GameExtensionId { get; set; }
        public GameExtension? GameExtension { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required int Order { get; set; }

        public int? NamePrefixId { get; set; }
        public InventoryEntryCategory? NamePrefix { get; set; }

        public List<InventoryEntryCategory> InventoryEntryCategories { get; set; } = [];

        public List<Participator> Participators { get; set; } = [];
        public List<ParticipatorInventoryEntry> ParticipatorInventoryEntries { get; set; } = [];

        public List<Weapon> Weapons { get; set; } = [];
        public List<WeaponCraftingIngredient> WeaponCraftingIngredients { get; set; } = [];

        public List<Armor> Armors { get; set; } = [];
        public List<ArmorCraftingIngredient> ArmorCraftingIngredients { get; set; } = [];
    }
}
