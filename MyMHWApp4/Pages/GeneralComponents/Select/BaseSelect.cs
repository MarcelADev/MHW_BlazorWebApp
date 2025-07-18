using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyMHWApp4.Pages.GeneralComponents.Select
{
    public abstract class BaseSelect<T> : ComponentBase, IAsyncDisposable
    {
        public string Id = $"mhw-{Guid.NewGuid():N}";

        protected string os = "";
        protected IJSObjectReference? module;
        protected readonly Dictionary<string, object> selectAttributes = [];
        protected readonly Dictionary<string, object> labelAttributes = [];

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = default!;

        /// <summary>
        /// Display this label string above the select.
        /// </summary>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>
        /// Display this placeholder string in the select.
        /// </summary>
        [Parameter]
        public string? Placeholder { get; set; }

        /// <summary>
        /// These options populate the select. Options MUST be passed!
        /// </summary>
        [Parameter]
        public required List<T> Options { get; set; }

        /// <summary>
        /// Used to map an option of type T to a value for the select option HTML element.
        /// </summary>
        [Parameter]
        public Func<T, object> OptionValueMapper { get; set; } = null!;

        /// <summary>
        /// Used to map an option of type T to a readable string.
        /// </summary>
        [Parameter]
        public Func<T, string> OptionNameMapper { get; set; } = null!;

        /// <summary>
        /// Disables interaction with the select.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        protected override async Task OnInitializedAsync()
        {
            os = await JSRuntime.InvokeAsync<string>("getOperatingSystem", []);
            StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            labelAttributes.Clear();

            if (!Disabled)
                labelAttributes["for"] = $"mhwapp-single-select-{Id}";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/select.js");
                await module.InvokeVoidAsync("addLabelHandlers", [$"{Id}-label"]);
            }
        }

        protected bool IsSelected(T option, T? currentValue)
        {
            return currentValue is not null && currentValue.Equals(option);
        }

        protected bool IsSelected(T option, List<T> currentValues)
        {
            var optionValue = OptionValueMapper(option);
            return currentValues.Any(v => OptionValueMapper(v)?.Equals(optionValue) == true);
        }

        public async ValueTask DisposeAsync()
        {
            if (module is not null)
            {
                try
                {
                    await module.DisposeAsync();
                }
                catch (JSDisconnectedException) { }
                GC.SuppressFinalize(this);
            }
        }
    }
}
