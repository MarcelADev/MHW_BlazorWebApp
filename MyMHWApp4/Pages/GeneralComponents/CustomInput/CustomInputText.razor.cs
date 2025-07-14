namespace MyMHWApp4.Pages.GeneralComponents.CustomInput
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using System.ComponentModel.DataAnnotations;

    public partial class CustomInputText : InputText, IDisposable
    {
        [Parameter] public string? Id { get; set; }

        private new string CssClass => $"custom-input {GetValidationClass()}";

        private string GetValidationClass()
        {
            if (EditContext == null)
                return string.Empty;

            bool hasError = EditContext.GetValidationMessages(FieldIdentifier).Any();
            bool isModified = EditContext.IsModified(FieldIdentifier);

            // Apply styling only if field has been interacted with
            return isModified ? (hasError ? "invalid" : "valid") : string.Empty;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (EditContext is not null)
            {
                EditContext.OnValidationStateChanged += HandleValidationStateChanged;
                EditContext.OnFieldChanged += HandleFieldChanged;
            }
        }

        private void HandleValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            if (e.FieldIdentifier.Equals(FieldIdentifier))
                InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (EditContext is not null)
            {
                EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
                EditContext.OnFieldChanged -= HandleFieldChanged;
            }
        }
    }
}
