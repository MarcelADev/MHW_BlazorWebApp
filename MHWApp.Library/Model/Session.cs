using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Session
    {
        public required int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Date { get; set; }

        [Required]
        public required int MaxDays { get; set; }

        [Required]
        public required int DayCount { get; set; }

        public List<Participator> Participators { get; set; } = [];

        
    }
}
