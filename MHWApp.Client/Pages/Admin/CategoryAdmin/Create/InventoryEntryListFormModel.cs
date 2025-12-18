using MHWApp.Library.Model;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace MHWApp.Client.Pages.Admin.CategoryAdmin.Create
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

        public Func<GameExtension> GetGeneralGameExtension { get; set; }
        public Action<GameExtension> SetGeneralGameExtension { get; set; }
        public GameExtension? GeneralGameExtension
        {
            get => GetGeneralGameExtension.Invoke();
            set => SetGeneralGameExtension.Invoke(value);
        }

        public List<string> CommonInventoryEntryNames = new List<string>
        {
            "Tail",
            "Fang",
            "Claw",
        };

        public List<InventoryEntryCardModel> CommonInventoryEntries { get; set; } = [];

        public List<InventoryEntryCardModel> SelectedCommonInventoryEntries { get; set; } = [];
        public List<InventoryEntryCardModel> UncommonInventoryentries { get; set; } = [];

        public string UncommonInventoryEntryName { get; set; } = string.Empty;

        public Action? UpdateCommonInventoryEntries { get; set; }

        public InventoryEntryListFormModel(Func<string> getName,Action<string> setName, Func<GameExtension> getGeneralGameExtension, Action<GameExtension> setGeneralGameExtension)
        {
            GetName = getName;
            SetName = setName;
            GetGeneralGameExtension = getGeneralGameExtension;
            SetGeneralGameExtension = setGeneralGameExtension;
        }

        private void OnUpdateCommonInventoryEntries()
        {
            if (UpdateCommonInventoryEntries is not null) UpdateCommonInventoryEntries.Invoke();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            Console.WriteLine($"🔍 Validate() called: Entry = '{UncommonInventoryEntryName}'");

            if (SelectedCommonInventoryEntries.Any(scie =>
                    scie.ExtendedName.Equals(UncommonInventoryEntryName, StringComparison.OrdinalIgnoreCase)) ||
                UncommonInventoryentries.Any(ucie =>
                    ucie.Name.Equals(UncommonInventoryEntryName, StringComparison.OrdinalIgnoreCase)))
            {
                yield return new ValidationResult("Inventory Entry is already in the list", [nameof(UncommonInventoryEntryName)]);
            }
            if (UncommonInventoryEntryName.Equals(string.Empty))
                yield return new ValidationResult("Inventory Entry name must not be empty", [nameof(UncommonInventoryEntryName)]);
        }
    }
}
