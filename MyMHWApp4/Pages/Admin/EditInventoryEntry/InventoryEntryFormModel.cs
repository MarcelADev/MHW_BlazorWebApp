using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditInventoryEntry
{
    public record InventoryEntryFormModel : IValidatableObject
    {
        public string Name { get; set; } = string.Empty;
        private List<InventoryEntry> _inventoryEntryList;

        public InventoryEntryCategory InventoryCategory { get; set; }

        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public InventoryEntryFormModel(List<InventoryEntry> inventoryEntryList)
        {
            _inventoryEntryList = inventoryEntryList;
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
            if (UnexpectedResponseFlag)
            {
                yield return new ValidationResult("Unexpected HTTP Response. Please contact Devs!", [nameof(Name)]);
            }
            if (Name.Trim() == "")
            {
                yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
            }
            if(_inventoryEntryList != null)
            {
                foreach(InventoryEntry entry in _inventoryEntryList)
                {
                    if(entry.Name.Equals(Name)) { yield return new ValidationResult("Inventory Entry already exists", [nameof(Name)]); }
                }
            }
        }
    }
}