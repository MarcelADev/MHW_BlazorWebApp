namespace MHWApp.Client.Pages.GeneralComponents.CustomInput
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Components.Rendering;
    using Microsoft.AspNetCore.Components.Web;
    using System.ComponentModel.DataAnnotations;

    public partial class CustomInputText : InputText, IDisposable
    {
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string InputType { get; set; } = "text";

        [Parameter]
        public string Placeholder { get; set; } = string.Empty;

        private new string CssClass => $"custom-input {GetValidationClass()}";

        private string GetValidationClass()
        {
            if (EditContext == null)
                return string.Empty;

            if (EditContext.GetValidationMessages(FieldIdentifier).Any())
                return "invalid";

            if (!EditContext.IsModified(FieldIdentifier))
                return string.Empty;

            return "valid";
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (EditContext is not null)
            {
                EditContext.OnValidationStateChanged += HandleValidationStateChanged;
                EditContext.OnFieldChanged += HandleFieldChanged; }
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
