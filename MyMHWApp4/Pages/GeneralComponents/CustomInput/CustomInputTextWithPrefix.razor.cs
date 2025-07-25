using Microsoft.AspNetCore.Components;

namespace MyMHWApp4.Pages.GeneralComponents.CustomInput
{
    public partial class CustomTextInputWithPrefix : ComponentBase
    {
        [Parameter] public string Prefix { get; set; } = string.Empty;
        [Parameter] public string? Id { get; set; }

        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? AdditionalAttributes { get; set; }
    }
}
