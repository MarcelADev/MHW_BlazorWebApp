using MHWApp.Library.Model;
using Microsoft.Extensions.Options;
using MHWApp.Client.Utils;
using System.ComponentModel.DataAnnotations;

namespace MHWApp.Client.Pages.Admin.Create
{
    public class CreateFormModel : IValidatableObject
    {
        [Required(ErrorMessage = "CurrentName must not be empty")]
        public string Name { get; set; } = string.Empty;

        public bool AddArmorSet { get; set; } = false;
        public bool AddInventoryEntries { get; set; } = false;
        public bool RequiresGameExtension => AddArmorSet || AddInventoryEntries;

        public bool UseAlternativeArmorPrefix { get; set; } = false;
        public string AlternativeArmorPrefix { get; set; } = string.Empty;
        public string ArmorPrefix => UseAlternativeArmorPrefix ? AlternativeArmorPrefix : Name;


        public string HelmetSuffix { get; set; } = "Helmet";
        public string MailSuffix { get; set; } = "Mail";
        public string GreavesSuffix { get; set; } = "Greaves";

        public GameExtension? GeneralGameExtension { get; set; }

        public bool ShowValidationMessages { get; set; } = true;
        
        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EntityAlreadyExistsFlag)
                yield return new ValidationResult("Category already exists!", [nameof(Name)]);

            if (UnexpectedResponseFlag)
                yield return new ValidationResult("Unexpected server response. Please reload page!", [nameof(Name)]);

            if ((AddArmorSet || AddInventoryEntries) && 
                GeneralGameExtension is null)
                yield return new ValidationResult("Game extension is needed for armor set and/or inventory entries", [nameof(GeneralGameExtension)]);

            if (AddArmorSet)
            {
                if (UseAlternativeArmorPrefix &&
                    AlternativeArmorPrefix == string.Empty)
                    yield return new ValidationResult("Alternative armor prefix can't be empty", [nameof(AlternativeArmorPrefix)]);

                if (HelmetSuffix == string.Empty)
                    yield return new ValidationResult("Helmet name can't be empty (prefix doesn't count)", [nameof(HelmetSuffix)]);

                if (MailSuffix == string.Empty)
                    yield return new ValidationResult("Mail name can't be empty (prefix doesn't count)", [nameof(MailSuffix)]);

                if (GreavesSuffix == string.Empty)
                    yield return new ValidationResult("Boots name can't be empty (prefix doesn't count)", [nameof(GreavesSuffix)]);
            }            
        }
    }
}
