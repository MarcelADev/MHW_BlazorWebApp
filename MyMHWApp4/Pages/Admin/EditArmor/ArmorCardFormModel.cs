using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditArmor
{
    public record ArmorCardFormModel : IValidatableObject
    {
        public string Name { get; set; }
        public string NewName { get; set; }

        public ArmorCategory CurrentArmorCategory { get; set; }
        public ArmorCategory SelectedArmorCategory { get; set; }

        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool EntityWasNotFoundFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;
        public bool CancelFlag { get; set; } = false;

        public ArmorCardFormModel(Armor armor)
        {
            Name = NewName = armor.Name;
            CurrentArmorCategory = armor.ArmorCategory!;
            SelectedArmorCategory = armor.ArmorCategory!;
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
                CurrentArmorCategory.Id == SelectedArmorCategory.Id)
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