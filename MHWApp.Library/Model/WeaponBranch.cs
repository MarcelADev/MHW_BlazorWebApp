using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class WeaponBranch
    {
        public int Id { get; set; }

        public int WeaponId { get; set; }
        public Weapon Weapon { get; set; }

        public int WeaponCategoryId { get; set; }
        public WeaponBranch WeaponCategory{ get; set; }

        [Required]
        public int Order { get; set; }
    }
}
