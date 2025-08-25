namespace MHWApp.Client.Pages.GeneralComponents.CustomInput
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    public partial class CustomInputCheckbox : InputCheckbox, IDisposable
    {
        [Parameter] public string? Id { get; set; }

        [Parameter] public int Hierarchy { get; set; } = 1;

        protected new string CssClass => $"custom-checkbox{GetValidationClass()}{GetHierarchyColor()}";

        private string GetValidationClass()
        {
            if (EditContext == null)
                return string.Empty;

            var isModified = EditContext.IsModified(FieldIdentifier);
            var hasError = EditContext.GetValidationMessages(FieldIdentifier).Any();

            if (!isModified)
                return string.Empty;

            return hasError ? " invalid" : " valid";
        }

        private string GetHierarchyColor()
        {
            if (Hierarchy == 1) return "";
            if (Hierarchy == 2) return " purple";

            return "";
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (EditContext is not null)
            {
                EditContext.OnValidationStateChanged += OnValidationStateChanged;
                EditContext.OnFieldChanged += OnFieldChanged;
            }
        }

        private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            if (e.FieldIdentifier.Equals(FieldIdentifier))
                InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (EditContext is not null)
            {
                EditContext.OnValidationStateChanged -= OnValidationStateChanged;
                EditContext.OnFieldChanged -= OnFieldChanged;
            }
        }
    }
}
