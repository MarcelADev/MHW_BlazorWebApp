using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditGameExtension
{
    public record GameExtensionFormModel : IValidatableObject
    {
        public string Name { get; set; } = string.Empty;
        private List<GameExtension> _gameExtensionList;

        public bool SideNeedsReload { get; set; } = false;

        public GameExtensionFormModel(List<GameExtension> gameExtensionList)
        {
            _gameExtensionList = gameExtensionList;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SideNeedsReload)
            {
                yield return new ValidationResult("Database and Client are unsynchronized. Please reload page!", [nameof(Name)]);
            }
            if (Name.Trim() == "")
            {
                yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
            }
            if(_gameExtensionList != null)
            {
                foreach(GameExtension gameExtension in _gameExtensionList)
                {
                    if(gameExtension.Name.Equals(Name)) { yield return new ValidationResult("Game Extension already exists", [nameof(Name)]); }
                }
            }
        }
    }
}