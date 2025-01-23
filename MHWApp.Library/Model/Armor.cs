using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Armor : IModel
    {
        public int Id { get; set; }

        public int ArmorCategoryId { get; set; }
        public required ArmorCategory ArmorCategory { get; set; }

        public int GameExtensionId { get; set; }
        public required GameExtension GameExtension { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public int Order { get; set; }

        public List<Participator> Participators { get; set; } = [];
        public List<ParticipatorArmor> ParticipatorArmors { get; set; } = [];

        public List<InventoryEntry> InventoryEntries { get; set; } = [];
        public List<ArmorCraftingIngredient> ArmorCraftingIngredients { get; set; } = [];
    }
}
