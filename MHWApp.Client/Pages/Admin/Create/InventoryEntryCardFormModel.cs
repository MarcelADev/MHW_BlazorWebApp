using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;

namespace MHWApp.Client.Pages.Admin.Create
{
    public class InventoryEntryCardFormModel : IValidatableObject
    {
        public string CurrentName { get; set; }
        public string SelectedName { get; set; }

        public GameExtension? CurrentGameExtension;

        [Required(ErrorMessage = "You must choose a game extension")]
        public GameExtension? SelectedGameExtension;

        public bool ShowValidationMessages { get; set; } = true;

        public bool NameAlreadyExistsFlag { get; set; } = false;

        public InventoryEntryCardFormModel(InventoryEntryCardModel model)
        {
            if (model.ExtendedName.Equals(string.Empty)) CurrentName = SelectedName = model.Name;
            else CurrentName = SelectedName = model.ExtendedName;
            
            CurrentGameExtension = SelectedGameExtension = model.GameExtension;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ShowValidationMessages)
            {
                if (CurrentName.Equals(SelectedName) && object.Equals(CurrentGameExtension, SelectedGameExtension))
                    yield return new ValidationResult("Nothing was changed", [nameof(SelectedName)]);

                if (NameAlreadyExistsFlag)
                    yield return new ValidationResult("This name already exists in the list", [nameof(SelectedName)]);
            }
        }
    }
}
