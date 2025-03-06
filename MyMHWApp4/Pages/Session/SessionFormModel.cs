using MHWApp.Library.Model;
using MyMHWApp4.Utils;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Session
{
    public record SessionFormModel : IValidatableObject
    {
        public string Name { get; set; } = string.Empty;
        public string Date { get; set; } = DateUtils.GetDateString();

        public int MaxDays { get; set; } = 1;
        public int DayCount { get; set; } = 1;

        public required Participator UserParticipator { get; set; }

        public int[] PlayerIds { get; set; } = [];
        public List<Participator> Participators { get; set; } = [];
        public List<Player> SelectedPlayers { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Name.Trim() == "")
            {
                yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
            }
            if (MaxDays < DayCount)
            {
                yield return new ValidationResult("Max days must be greater than or equal to day count", [nameof(MaxDays)]);
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
