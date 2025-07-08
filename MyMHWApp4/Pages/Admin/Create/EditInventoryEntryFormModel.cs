using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Admin.Create
{
    public class EditInventoryEntryFormModel : IValidatableObject
    {
        public string CurrentName { get; set; }
        public string SelectedName { get; set; }

        public GameExtension? CurrentGameExtension;
        public GameExtension? SelectedGameExtension;

        public EditInventoryEntryFormModel(InventoryEntryCardModel model)
        {
            CurrentName = SelectedName = model.Name;
            CurrentGameExtension = SelectedGameExtension = model.GameExtension;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(CurrentName.Equals(SelectedName))
                yield return new ValidationResult("Nothing was changed", [nameof(CurrentName)]);
        }
    }
}
