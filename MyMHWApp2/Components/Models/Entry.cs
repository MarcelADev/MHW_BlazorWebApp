using System.ComponentModel.DataAnnotations;

namespace MyMHWApp2.Components.Models
{
    public class Entry
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
