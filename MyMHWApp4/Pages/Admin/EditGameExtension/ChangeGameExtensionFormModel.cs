using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditGameExtension
{
    public record ChangeGameExtensionFormModel : IValidatableObject
    {
        public string Name { get; set; }
        public string NewName { get; set; }

        public ChangeGameExtensionFormModel(GameExtension gameExtension)
        {
            Name = NewName = gameExtension.Name;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NewName.Trim() == "")
            {
                yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
            }
            if (Name.Equals(NewName))
            {
                yield return new ValidationResult("Name was not changed", [nameof(Name)]);
            }
        }
    }
}