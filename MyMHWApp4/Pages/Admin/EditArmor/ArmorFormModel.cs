using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyMHWApp4.Pages.Admin.EditArmor
{
    public record ArmorFormModel : IValidatableObject
    {
        public string Name { get; set; } = string.Empty;
        private List<Armor> _armorList;

        public ArmorCategory ArmorCategory { get; set; }

        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

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
            if(_armorList != null)
            {
                foreach(Armor armor in _armorList)
                {
                    if(armor.Name.Equals(Name)) { yield return new ValidationResult("Armor already exists", [nameof(Name)]); }
                }
            }
        }
    }
}