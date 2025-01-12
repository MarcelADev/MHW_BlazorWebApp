using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Session
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Date { get; set; }
    }
}
