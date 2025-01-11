using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Player
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
