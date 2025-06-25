using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.GeneralComponents.Modals.EditSessionModal
{
    public class EditSessionFormModel : IValidatableObject
    {
        public string CurrentName { get; set; }
        public string NewName { get; set; }

        public int CurrentDayCount { get; set; }
        public int NewDayCount { get; set; }

        public int CurrentMaxDays { get; set; }
        public int NewMaxDays { get; set; }

        public int[] PlayerIds { get; set; } = [];

        public List<Participator> RemoveParticipators { get; set; } = [];
        public List<Participator> AddParticipators { get; set; } = [];

        public bool EntityWasNullFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public EditSessionFormModel(string sessionName, int dayCount, int maxDays)
        {
            CurrentName = NewName = sessionName;
            CurrentDayCount = NewDayCount = dayCount;
            CurrentMaxDays = NewMaxDays = maxDays;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EntityWasNullFlag)
            {
                yield return new ValidationResult("Contradictory HTTP Response. Please contact Server Provider!", [nameof(CurrentName)]);
            }
            if (UnexpectedResponseFlag)
            {
                yield return new ValidationResult("Unexpected HTTP Response. Please contact Devs!", [nameof(CurrentName)]);
            }
            if (CurrentName.Trim() == "")
            {
                yield return new ValidationResult("CurrentName must not be empty", [nameof(CurrentName)]);
            }
            if (NewName == CurrentName &&
                !PlayerIds.Any() &&
                !RemoveParticipators.Any() &&
                NewDayCount == CurrentDayCount &&
                NewMaxDays == CurrentMaxDays)
            {
                yield return new ValidationResult("Nothing was changed!");
            }
        }
    }
}
