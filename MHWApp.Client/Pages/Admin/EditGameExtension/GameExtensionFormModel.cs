using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MHWApp.Client.Pages.Admin.EditGameExtension
{
    public record GameExtensionFormModel : IValidatableObject
    {
        public string Name { get; set; } = string.Empty;
        private List<GameExtension> _gameExtensionList;

        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public GameExtensionFormModel(List<GameExtension> gameExtensionList)
        {
            _gameExtensionList = gameExtensionList;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EntityAlreadyExistsFlag)
            {
                yield return new ValidationResult("Database and Client are unsynchronized. Please reload page!", [nameof(Name)]);
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