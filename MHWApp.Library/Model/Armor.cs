using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Armor
    {
        public required int Id { get; set; }

        [Required]
        public required int GameExtensionId { get; set; }
        public GameExtension? GameExtension { get; set; }

        public required int ArmorCategoryId { get; set; }
        public ArmorCategory? ArmorCategory { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required int Order { get; set; }

        public bool NamePrefix { get; set; } = false;

        public List<Participator> Participators { get; set; } = [];
        public List<ParticipatorArmor> ParticipatorArmors { get; set; } = [];

        public List<InventoryEntry> InventoryEntries { get; set; } = [];
        public List<ArmorCraftingIngredient> ArmorCraftingIngredients { get; set; } = [];
    }
}
