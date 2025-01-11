using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Session
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Date { get; set; }
    }
}
