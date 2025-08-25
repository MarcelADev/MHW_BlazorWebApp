using MHWApp.Library.Model;
using MHWApp.Client.Utils;
using System.ComponentModel.DataAnnotations;

namespace MHWApp.Client.Pages.MainMenu.CreateSession
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

        public bool BadRequestFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BadRequestFlag)
            {
                yield return new ValidationResult("DTO was incomplete! Please contact Devs!", [nameof(Name)]);
            }
            if (EntityWasNullFlag)
            {
                yield return new ValidationResult("Contradictory HTTP Response. Please contact Server Provider!", [nameof(Name)]);
            }
            if (UnexpectedResponseFlag)
            {
                yield return new ValidationResult("Unexpected HTTP Response. Please contact Devs!", [nameof(Name)]);
            }
            if (Name.Trim() == "")
            {
                yield return new ValidationResult("CurrentName must not be empty", [nameof(Name)]);
            }
            if (MaxDays < DayCount)
            {
                yield return new ValidationResult("Max days must be greater than or equal to day count", [nameof(MaxDays)]);
            }
        }
    }
}
