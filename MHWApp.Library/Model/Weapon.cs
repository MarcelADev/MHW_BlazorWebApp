using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Weapon
    {
        public int Id { get; set; }

        public int WeaponTypeId { get; set; }
        public WeaponType WeaponType { get; set; }

        public int GameExtensionId { get; set; }
        public GameExtension GameExtension { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int WeaponTier { get; set; }
    }
}
