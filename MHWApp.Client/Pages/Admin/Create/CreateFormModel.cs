using MHWApp.Library.Model;
using Microsoft.Extensions.Options;
using MHWApp.Client.Utils;
using System.ComponentModel.DataAnnotations;

namespace MHWApp.Client.Pages.Admin.Create
{
    public class CreateFormModel : IValidatableObject
    {
        [Required(ErrorMessage = "Name must not be empty")]
        public string Name { get; set; } = string.Empty;

        public bool AddArmorSet { get; set; } = false;
        public bool AddInventoryEntries { get; set; } = false;
        public bool RequiresGameExtension => AddArmorSet || AddInventoryEntries;

        public bool UseAlternativeArmorPrefix { get; set; } = false;

        [RequiredIf("UseAlternativeArmorPrefix", true, ErrorMessage = "Alternative Prefix must not be empty")]
        public string AlternativeArmorPrefix { get; set; } = string.Empty;
        public string ArmorPrefix => UseAlternativeArmorPrefix ? AlternativeArmorPrefix : Name;


        [RequiredIf("AddArmorSet", true, ErrorMessage = "Helmet Suffix must not be empty")]
        public string HelmetSuffix { get; set; } = "Helmet";

        [RequiredIf("AddArmorSet", true, ErrorMessage = "Mail Suffix must not be empty")]
        public string MailSuffix { get; set; } = "Mail";

        [RequiredIf("AddArmorSet", true, ErrorMessage = "Greaves Suffix must not be empty")]
        public string GreavesSuffix { get; set; } = "Greaves";

        //[RequiredIf("RequiresGameExtension", true, ErrorMessage = "Game extension is needed for armor set and/or inventory entries")]
        public GameExtension? GeneralGameExtension { get; set; }

        public bool ShowValidationMessages { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((AddArmorSet || AddInventoryEntries) && 
                GeneralGameExtension is null)
                yield return new ValidationResult("Game extension is needed for armor set and/or inventory entries", [nameof(GeneralGameExtension)]);
        }
    }
}
