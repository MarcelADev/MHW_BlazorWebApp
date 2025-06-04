using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.GeneralComponents.Modals.EditSessionModal
{
    public class EditSessionFormModel : IValidatableObject
    {
        public string CurrentName { get; set; }
        public string NewName { get; set; }

        public List<Player> SelectedNewPlayers { get; set; } = [];

        public List<Participator> CurrentParticipators { get; set; } = [];
        public List<Participator> SelectedParticipators { get; set; } = [];

        public EditSessionFormModel(string sessionName, List<Player> players, List<Participator> participators)
        {
            CurrentName = NewName = sessionName;

            foreach(Participator p in participators)
            {
                Participator participatorClone = p.Clone();
                participatorClone.Id = p.Id;
                participatorClone.Player = p.Player;

                CurrentParticipators.Add(participatorClone);
                SelectedParticipators.Add(participatorClone);
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentName.Trim() == "")
            {
                yield return new ValidationResult("CurrentName must not be empty", [nameof(CurrentName)]);
            }
            if (NewName == CurrentName &&
                !SelectedNewPlayers.Any() &&
                CurrentParticipators.All(cp => SelectedParticipators.Contains(cp)) &&
                SelectedParticipators.All(cp => CurrentParticipators.Contains(cp)))
            {
                yield return new ValidationResult("Nothing was changed!");
            }
        }
    }
}
