using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditCategory
{
    public record CategoryCardFormModel : IValidatableObject
    {
        public string Name { get; set; }
        public string NewName { get; set; }

        public bool IsInventoryCategory { get; set; }
        public bool IsWeaponCategory { get; set; }
        public bool IsArmorCategory { get; set; }

        public bool IsCurrentlyInventoryCategory { get; set; }
        public bool IsCurrentlyWeaponCategory { get; set; }
        public bool IsCurrentlyArmorCategory { get; set; }

        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool EntityWasNotFoundFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;
        public bool CancelFlag { get; set; } = false;

        public GameExtension CurrentGameExtension { get; set; }
        public GameExtension NewGameExtension { get; set; }

        public CategoryCardFormModel(Category category)
        {
            Name = NewName = category.Name;
            IsCurrentlyInventoryCategory = IsInventoryCategory = category.InventoryEntryCategory != null;
            IsCurrentlyWeaponCategory = IsWeaponCategory = category.WeaponCategory != null;
            IsCurrentlyArmorCategory = IsArmorCategory = category.ArmorCategory != null;
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
                yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
            }
            if (Name.Equals(NewName) && 
                IsInventoryCategory == IsCurrentlyInventoryCategory &&
                IsWeaponCategory == IsCurrentlyWeaponCategory &&
                IsArmorCategory == IsCurrentlyArmorCategory &&
                CurrentGameExtension.Id == NewGameExtension.Id)
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