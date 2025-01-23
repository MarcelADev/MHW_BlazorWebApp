using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Weapon : IModel
    {
        public int Id { get; set; }

        public int WeaponCategoryId { get; set; }
        public required WeaponCategory WeaponCategory { get; set; }

        public int WeaponTypeId { get; set; }
        public required WeaponType WeaponType { get; set; }

        public int GameExtensionId { get; set; }
        public required GameExtension GameExtension { get; set; }

        public int PredecessorId { get; set; }
        public Weapon? Predecessor { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public int WeaponTier { get; set; }

        [Required]
        public int Order { get; set; }

        public List<Participator> Participators { get; set; } = [];     // Weapon and Participator have the many to many relationship
        public List<ParticipatorWeapon> ParticipatorWeapons { get; set; } = [];     // Joint table of Weapon and Participator

        public List<InventoryEntry> InventoryEntries { get; set; } = [];
        public List<WeaponCraftingIngredient> WeaponCraftingIngredients { get; set; } = [];
    }
}
