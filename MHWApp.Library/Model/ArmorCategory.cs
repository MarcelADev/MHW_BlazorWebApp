using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class ArmorCategory
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public required Category Category { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
