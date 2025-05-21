using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.GeneralComponents.Modals.EditSessionModal
{
    public class AddParticipatorFormModel : IValidatableObject
    {
        public List<Player> CurrentPlayers { get; set; } = [];
        public List<Player> SelectedPlayers { get; set; } = [];

        public AddParticipatorFormModel(List<Player> players)
        {
            CurrentPlayers = players.ToList();
            SelectedPlayers = players;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentPlayers.All(cp => SelectedPlayers.Contains(cp)) &&
                SelectedPlayers.All(cp => CurrentPlayers.Contains(cp)))
            {
                yield return new ValidationResult("Nothing was changed!");
            }
        }
    }
}
