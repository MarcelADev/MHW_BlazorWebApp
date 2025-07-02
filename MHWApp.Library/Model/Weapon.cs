using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Weapon
    {
        public required int Id { get; set; }

        public required int WeaponCategoryId { get; set; }
        public WeaponCategory? WeaponCategory { get; set; }

        public required int WeaponTypeId { get; set; }
        public WeaponType? WeaponType { get; set; }

        public required int PredecessorId { get; set; }
        public Weapon? Predecessor { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required int WeaponTier { get; set; }

        [Required]
        public required int Order { get; set; }

        public List<Participator> Participators { get; set; } = [];     // Weapon and Participator have the many to many relationship
        public List<ParticipatorWeapon> ParticipatorWeapons { get; set; } = [];     // Joint table of Weapon and Participator

        public List<InventoryEntry> InventoryEntries { get; set; } = [];
        public List<WeaponCraftingIngredient> WeaponCraftingIngredients { get; set; } = [];
    }
}
