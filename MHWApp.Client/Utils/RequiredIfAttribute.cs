using System.ComponentModel.DataAnnotations;

namespace MHWApp.Client.Utils
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _dependentProperty;
        private readonly object? _targetValue;

        public RequiredIfAttribute(string dependentProperty, object? targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var containerType = validationContext.ObjectInstance.GetType();
            var field = containerType.GetProperty(_dependentProperty);

            if (field == null)
                return new ValidationResult($"Unknown property: {_dependentProperty}");

            var dependentValue = field.GetValue(validationContext.ObjectInstance, null);

            if (Equals(dependentValue, _targetValue))
            {
                if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
