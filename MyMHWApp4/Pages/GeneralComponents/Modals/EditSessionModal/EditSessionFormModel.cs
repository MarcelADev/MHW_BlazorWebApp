using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.GeneralComponents.Modals.EditSessionModal
{
    public class EditSessionFormModel : IValidatableObject
    {
        public string CurrentName { get; set; }
        public string NewName { get; set; }

        public List<Player> CurrentPlayers { get; set; } = [];
        public List<Player> SelectedPlayers { get; set; } = [];

        public List<Participator> CurrentParticipators { get; set; } = [];
        public List<Participator> SelectedParticipators { get; set; } = [];

        public EditSessionFormModel(string sessionName, List<Player> players, List<Participator> participators)
        {
            CurrentName = NewName = sessionName;

            CurrentPlayers = players.ToList();
            SelectedPlayers = players;

            CurrentParticipators = participators.ToList();
            SelectedParticipators = participators;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentName.Trim() == "")
            {
                yield return new ValidationResult("CurrentName must not be empty", [nameof(CurrentName)]);
            }
            if (NewName == CurrentName &&
                CurrentPlayers.All(cp => SelectedPlayers.Contains(cp)) &&
                SelectedPlayers.All(cp => CurrentPlayers.Contains(cp)) &&
                CurrentParticipators.All(cp => SelectedParticipators.Contains(cp)) &&
                SelectedParticipators.All(cp => CurrentParticipators.Contains(cp)))
            {
                yield return new ValidationResult("Nothing was changed!");
            }
        }
    }
}
