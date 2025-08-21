using MHWApp.Library.Model;
using MyMHWApp4.Utils;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Admin.Create
{
    public class CreateFormModel
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

        [RequiredIf("RequiresGameExtension", true, ErrorMessage = "Game extension is needed for armorset and/or inventory entries")]
        public GameExtension? GeneralGameExtension { get; set; }

        public bool ShowValidationMessages { get; set; } = true;
    }
}
