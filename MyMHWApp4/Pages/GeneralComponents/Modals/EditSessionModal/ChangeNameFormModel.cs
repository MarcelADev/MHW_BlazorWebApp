using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.GeneralComponents.Modals.EditSessionModal
{
    public class ChangeNameFormModel : IValidatableObject
    {
        public string CurrentName { get; set; }
        public string NewName { get; set; }

        public ChangeNameFormModel(string sessionName)
        {
            CurrentName = NewName = sessionName;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentName.Trim() == "")
            {
                yield return new ValidationResult("CurrentName must not be empty", [nameof(CurrentName)]);
            }
        }
    }
}
