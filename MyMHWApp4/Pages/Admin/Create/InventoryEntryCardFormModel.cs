using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Admin.Create
{
    public class InventoryEntryCardFormModel : IValidatableObject
    {
        public string CurrentName { get; set; }
        public string SelectedName { get; set; }

        public GameExtension? CurrentGameExtension;
        public GameExtension? SelectedGameExtension;

        public bool ShowValidationMessages { get; set; } = true;

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
                    yield return new ValidationResult("Nothing was changed", [nameof(CurrentName)]);
            }
        }
    }
}
