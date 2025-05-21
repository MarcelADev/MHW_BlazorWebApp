using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.GeneralComponents.Modals.EditSessionModal
{
    public class RemoveParticipatorFormModel : IValidatableObject
    {
        public List<Participator> CurrentParticipators { get; set; } = [];
        public List<Participator> SelectedParticipators{ get; set; } = [];

        public RemoveParticipatorFormModel(List<Participator> participators)
        {
            CurrentParticipators = participators.ToList();
            SelectedParticipators = participators;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentParticipators.All(cp => SelectedParticipators.Contains(cp)) &&
                SelectedParticipators.All(cp => CurrentParticipators.Contains(cp)))
            {
                yield return new ValidationResult("Nothing was changed!");
            }
        }
    }
}
