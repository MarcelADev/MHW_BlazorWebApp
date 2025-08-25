using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MHWApp.Client.Pages.Admin.EditGameExtension
{
    public record GameExtensionCardFormModel : IValidatableObject
    {
        public string Name { get; set; }
        public string NewName { get; set; }

        public bool EntityWasNotFoundFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public GameExtensionCardFormModel(GameExtension gameExtension)
        {
            Name = NewName = gameExtension.Name;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EntityWasNullFlag)
            {
                yield return new ValidationResult("Contradictory HTTP Response. Please contact Server Provider!", [nameof(Name)]);
            }
            if (EntityWasNotFoundFlag)
            {
                yield return new ValidationResult("Couldnt remove Entity because it wasn't found. Please reload page!", [nameof(Name)]);
            }
            if (UnexpectedResponseFlag)
            {
                yield return new ValidationResult("Unexpected HTTP Response. Please contact Devs!", [nameof(Name)]);
            }
            if (NewName.Trim() == "")
            {
                yield return new ValidationResult("CurrentName must not be empty", [nameof(Name)]);
            }
            if (Name.Equals(NewName))
            {
                yield return new ValidationResult("CurrentName was not changed", [nameof(Name)]);
            }
        }
    }
}