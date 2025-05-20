using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Admin.EditCategory
{
    public class CategoryFormModel : IValidatableObject
    {
        private List<Category> _categoryList;

        public string Name { get; set; } = string.Empty;

        public bool IsInventoryCategory { get; set; } = false;
        public bool IsWeaponCategory { get; set; } = false;
        public bool IsArmorCategory { get; set; } = false;
        

        public bool EntityAlreadyExistsFlag { get; set; } = false;
        public bool EntityWasNullFlag { get; set; } = false;
        public bool UnexpectedResponseFlag { get; set; } = false;

        public GameExtension? GameExtension { get; set; }

        public CategoryFormModel(List<Category> categoryList, GameExtension? gameExtension)
        {
            _categoryList = categoryList;
            GameExtension = gameExtension;
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
            if (_categoryList != null)
            {
                foreach (Category category in _categoryList)
                {
                    if (category.Name.Equals(Name)) { yield return new ValidationResult("Category already exists", [nameof(Name)]); }
                }
            }
        }
    }
}
