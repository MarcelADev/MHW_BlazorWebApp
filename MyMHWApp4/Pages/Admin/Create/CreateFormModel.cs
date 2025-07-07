using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Pages.Admin.Create
{
    public class CreateFormModel : IValidatableObject
    {
        private string name = string.Empty;
        public string Name { 
            get => name;
            set
            {
                name = value;
                Helmet = name + " Helmet";
                Mail = name + " Mail";
                Boots = name + " Boots";
            }
        }

        public string Helmet { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Boots { get; set; } = string.Empty;

        public List<string> SelectedCommonInventoryEntries = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Name.Equals(string.Empty))
                yield return new ValidationResult("Name must not be empty", [nameof(Name)]);
        }
    }
}
