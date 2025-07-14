using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MyMHWApp4.Pages.GeneralComponents.Select
{
    public partial class SingleSelect<T> : ComponentBase
    {
        public string Id = $"mhw-{Guid.NewGuid():N}";

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
        /// Bind to this value in case of single select.
        /// </summary>
        [Parameter]
        public T? Value { get; set; }

        /// <summary>
        /// Event callback for single value select changes.
        /// </summary>
        /// <value></value>
        [Parameter]
        public EventCallback<T?> ValueChanged { get; set; }

        /// <summary>
        /// This Option applies an alternate style to the label
        /// </summary>
        [Parameter]
        public bool MutedLabel { get; set; } = false;

        /// <summary>
        /// These options populate the select.
        /// </summary>
        [Parameter, EditorRequired]
        public required List<T> Options { get; set; } = [];

        private Type? _optionType;

        /// <summary>
        /// Used to map an option of type T to a value for the select option HTML element.
        /// </summary>
        [Parameter, EditorRequired]
        public Func<T, object> OptionValueMapper { get; set; } = null!;

        /// <summary>
        /// Used to map an option of type T to a readable string.
        /// </summary>
        [Parameter, EditorRequired]
        public Func<T, string> OptionNameMapper { get; set; } = null!;

        private string os = "";

        private readonly Dictionary<string, object> selectAttributes = [];

        private string labelClass = string.Empty;

        private IJSObjectReference? module;

        private readonly Dictionary<string, object> labelAttributes = new();

        /// <summary>
        /// Disables interaction with the select.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            os = await JSRuntime.InvokeAsync<string>("getOperatingSystem", []);
            if (MutedLabel)
                labelClass = "muted";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./js/select.js"
                );

                await module.InvokeVoidAsync("addLabelHandlers", [$"{Id}-label"]);
            }
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
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

        protected override void OnParametersSet()
        {
            if (Options.Count > 0)
            {
                _optionType = OptionValueMapper(Options[0]).GetType();
            }
            else
            {
                _optionType = null;
            }

            labelAttributes.Clear();

            if (!Disabled)
            {
                labelAttributes["for"] = $"mhwapp-single-select-{Id}";
            }
        }

        private void HandleNativeChange(ChangeEventArgs args)
        {
            if (null == args || null == args.Value)
                return;

            if (_optionType == typeof(int))
            {
                HandleNativeChangeSingle(int.Parse((string)args.Value));
            }
            else if (_optionType == typeof(string))
            {
                HandleNativeChangeSingle((string)args.Value);
            }
        }

        private void HandleNativeChangeSingle(int id)
        {
            Value = Options.Find(o => (int)OptionValueMapper(o) == id);
            ValueChanged.InvokeAsync(Value);
        }

        private void HandleNativeChangeSingle(string id)
        {
            Value = Options.Find(o => (string)OptionValueMapper(o) == id);
            ValueChanged.InvokeAsync(Value);
        }

        private void HandleCustomChange(T option)
        {
            if (!Equals(Value, option))
            {
                Value = option;
                ValueChanged.InvokeAsync(Value);
            }
        }

        private bool IsSelected(T option)
        {
            return Value is not null && Value.Equals(option);
        }
    }
}
