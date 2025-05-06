using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditCategory
{
    public record CategoryCardFormModel : IValidatableObject
    {
        public string Name { get; set; }
        public string NewName { get; set; }

        public bool IsInventoryCategory { get; set; } = false;
        public bool IsWeaponCategory { get; set; } = false;
        public bool IsArmorCategory { get; set; } = false;

        public CategoryCardFormModel(Category category)
        {
            Name = NewName = category.Name;
            IsInventoryCategory = category.InventoryEntryCategory != null;
            IsWeaponCategory = category.WeaponCategory != null;
            IsArmorCategory = category.ArmorCategory != null;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NewName.Trim() == "")
            {
                yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
            }
            if (Name.Equals(NewName))
            {
                yield return new ValidationResult("Name was not changed", [nameof(Name)]);
            }
        }
    }
}