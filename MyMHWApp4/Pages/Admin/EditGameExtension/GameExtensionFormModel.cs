using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditGameExtension
{
    public record GameExtensionFormModel : IValidatableObject
    {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.Trim() == "")
            {
                yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
            }
        }
    }
}