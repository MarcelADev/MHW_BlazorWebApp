using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace MyMHWApp4.Utils
{
    public class ManualModelValidator
    {
        public static void ValidateModel(EditContext editContext, object model, ValidationMessageStore messageStore)
        {
            messageStore.Clear();

            var validationResults = new HashSet<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Validate data annotations
            Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Explicitly call IValidatableObject.Validate if implemented
            if (model is IValidatableObject validatable)
            {
                var customResults = validatable.Validate(validationContext);

                foreach (ValidationResult customResult in customResults)
                {
                    validationResults.Add(customResult);
                }
            }

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    messageStore.Add(editContext.Field(memberName), validationResult.ErrorMessage ?? "");
                }
            }

            editContext.NotifyValidationStateChanged();
        }
    }
}
