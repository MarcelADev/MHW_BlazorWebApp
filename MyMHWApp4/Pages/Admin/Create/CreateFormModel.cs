using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Admin.Create
{
    public class CreateFormModel : IValidatableObject
    {
        private string name = string.Empty;
        public string Name { 
            get => name;
            set
            {
                name = value;
                Helmet = name + " Helmet";
                Mail = name + " Mail";
                Boots = name + " Boots";
                OnUpdateCommonInventoryEntries();
            }
        }

        public string Helmet { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Boots { get; set; } = string.Empty;

        public GameExtension? GeneralGameExtension { get; set; }

        public List<InventoryEntryCardModel> SelectedCommonInventoryEntries = [];

        public string UncommonInventoryEntryName { get; set; } = string.Empty;

        public bool ShowValidationMessages { get; set; } = true;

        public Action? UpdateCommonInventoryEntries { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ShowValidationMessages)
            {
                if (Name.Equals(string.Empty))
                    yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
            }
        }

        private void OnUpdateCommonInventoryEntries()
        {
            if (UpdateCommonInventoryEntries is not null) UpdateCommonInventoryEntries.Invoke();
        }
    }
}
