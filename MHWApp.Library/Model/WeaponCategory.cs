using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class WeaponCategory
    {
        public required int Id { get; set; }

        public required int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public required int Order { get; set; }
    }
}
