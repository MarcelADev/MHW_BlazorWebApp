using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class GameExtension : IModel
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }
    }
}
