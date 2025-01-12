using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class WeaponType
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }
    }
}