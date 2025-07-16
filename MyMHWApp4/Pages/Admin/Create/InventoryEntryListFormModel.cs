using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Admin.Create
{
    public class InventoryEntryListFormModel : IValidatableObject
    {
        public Func<string> GetName { get; set; }
        public Action<string> SetName { get; set; }
        public string Name
        {
            get => GetName.Invoke();
            set
            {
                SetName.Invoke(value);
                OnUpdateCommonInventoryEntries();
            }
        }

        public List<string> CommonInventoryEntryNames = new List<string>
        {
            "Tail",
            "Fang",
            "Claw",
        };

        public List<InventoryEntryCardModel> CommonInventoryEntries { get; set; } = [];

        public List<InventoryEntryCardModel> SelectedCommonInventoryEntries = [];
        public List<InventoryEntryCardModel> UncommonInventoryentries = [];

        public string UncommonInventoryEntryName { get; set; } = string.Empty;

        public GameExtension? GeneralGameExtension { get; set; }

        public Action? UpdateCommonInventoryEntries { get; set; }

        public InventoryEntryListFormModel(Func<string> getName,Action<string> setName)
        {
            GetName = getName;
            SetName = setName;
        }

        private void OnUpdateCommonInventoryEntries()
        {
            if (UpdateCommonInventoryEntries is not null) UpdateCommonInventoryEntries.Invoke();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SelectedCommonInventoryEntries.Any(scie => scie.ExtendedName.Equals(UncommonInventoryEntryName)) ||
                UncommonInventoryentries.Any(ucie => ucie.Name.Equals(UncommonInventoryEntryName)))
                yield return new ValidationResult("Inventory Entry is already in the list", [nameof(UncommonInventoryEntryName)]);
        }
    }
}
