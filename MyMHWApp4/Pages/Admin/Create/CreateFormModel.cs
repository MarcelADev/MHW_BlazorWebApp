using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Admin.Create
{
    public class CreateFormModel
    {
        [Required(ErrorMessage = "Name must not be empty")]
        public string Name { get; set; } = string.Empty;

        public bool UseAlternativeArmorPrefix { get; set; } = false;

        [Required(ErrorMessage = "Alternative Prefix must not be empty")]
        public string AlternativeArmorPrefix { get; set; } = string.Empty;
        public string ArmorPrefix => UseAlternativeArmorPrefix ? AlternativeArmorPrefix : Name;


        [Required(ErrorMessage = "Helmet Suffix must not be empty")]
        public string HelmetSuffix { get; set; } = "Helmet";

        [Required(ErrorMessage = "Mail Suffix must not be empty")]
        public string MailSuffix { get; set; } = "Mail";

        [Required(ErrorMessage = "Greaves Suffix must not be empty")]
        public string GreavesSuffix { get; set; } = "Greaves";

        public GameExtension? GeneralGameExtension { get; set; }

        public bool ShowValidationMessages { get; set; } = true;
    }
}
