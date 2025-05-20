using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.MainMenu
{
    public class SessionCardFormModel : IValidatableObject
    {
        public bool ParticipatorWasNotFoundFlag { get; set; } = false;
        public bool SessionWasNotFoundFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ParticipatorWasNotFoundFlag)
            {
                yield return new ValidationResult("Couldnt remove Participator because it wasn't found. Please reload page!");
            }
            if (SessionWasNotFoundFlag)
            {
                yield return new ValidationResult("Couldnt remove Session because it wasn't found. Please reload page!");
            }
            if (UnexpectedResponseFlag)
            {
                yield return new ValidationResult("Unexpected HTTP Response. Please contact Devs!");
            }
        }
    }
}
