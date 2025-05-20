using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditCategory
{
    public record InventoryEntryCardFormModel : IValidatableObject
    {
        public string Name { get; set; }
        public string NewName { get; set; }

        public List<InventoryEntryCategory> CurrentInventoryCategories { get; set; } = [];
        public List<InventoryEntryCategory> SelectedInventoryCategories { get; set; } = [];

        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool EntityWasNotFoundFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;
        public bool CancelFlag { get; set; } = false;

        public InventoryEntryCardFormModel(InventoryEntry inventoryEntry)
        {
            Name = NewName = inventoryEntry.Name;
            CurrentInventoryCategories = inventoryEntry.InventoryEntryCategories.ToList();
            SelectedInventoryCategories = inventoryEntry.InventoryEntryCategories;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EntityAlreadyExistsFlag)
            {
                yield return new ValidationResult("Database and Client are unsynchronized. Please reload page!", [nameof(Name)]);
            }
            if (EntityWasNullFlag)
            {
                yield return new ValidationResult("Contradictory HTTP Response. Please contact Server Provider!", [nameof(Name)]);
            }
            if (EntityWasNotFoundFlag)
            {
                yield return new ValidationResult("Couldnt remove Entity because it wasn't found. Please reload page!", [nameof(Name)]);
            }
            if (UnexpectedResponseFlag)
            {
                yield return new ValidationResult("Unexpected HTTP Response. Please contact Devs!", [nameof(Name)]);
            }
            if (NewName.Trim() == "")
            {
                yield return new ValidationResult("CurrentName must not be empty", [nameof(Name)]);
            }
            if (Name.Equals(NewName) &&
                CurrentInventoryCategories.All(iec => SelectedInventoryCategories.Contains(iec)) &&
                SelectedInventoryCategories.All(iec => CurrentInventoryCategories.Contains(iec)))
            {
                yield return new ValidationResult("Nothing was changed", [nameof(Name)]);
            }
            if (CancelFlag)
            {
                yield return new ValidationResult("Unexpected Error ocurred and the process got canceled!", [nameof(Name)]);
            }
        }
    }
}