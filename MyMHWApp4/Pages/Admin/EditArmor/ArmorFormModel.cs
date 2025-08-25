using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;
using MHWApp.Client.Pages.GeneralComponents.Select;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MHWApp.Client.Pages.Admin.EditArmor
{
    public record ArmorFormModel : IValidatableObject
    {
        public string Name { get; set; } = string.Empty;
        private List<Armor> _armorList;

        public GameExtension? GameExtension { get; set; }
        public ArmorCategory? ArmorCategory { get; set; }

        public bool UseNamePrefix { get; set; }

        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool BadRequestFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public BadRequestException? BadRequestException { get; set; }

        public ArmorFormModel(List<Armor> armorList)
        {
            _armorList = armorList;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EntityAlreadyExistsFlag)
            {
                yield return new ValidationResult("Database and Client are unsynchronized. Please reload page!", [nameof(Name)]);
            }
            if(BadRequestFlag)
            {
                yield return new ValidationResult(BadRequestException!.Message, [nameof(Name)]);
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
                yield return new ValidationResult("CurrentName must not be empty", [nameof(Name)]);
            }
            if (_armorList != null)
            {
                string formArmorName = "";
                if (ArmorCategory is not null && UseNamePrefix) formArmorName += ArmorCategory.Category!.Name + " ";
                formArmorName += Name;

                foreach (Armor armor in _armorList)
                {
                    string existingArmorName = "";
                    if (armor.NamePrefix) existingArmorName += armor.ArmorCategory!.Category!.Name + " ";
                    existingArmorName += armor.Name;

                    if (existingArmorName.Equals(formArmorName, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new ValidationResult("Armor already exists", [nameof(Name)]);
                    }
                }
            }
            if (ArmorCategory is null)
            {
                yield return new ValidationResult("You must choose a category!", [nameof(Name)]);
            }
        }
    }
}