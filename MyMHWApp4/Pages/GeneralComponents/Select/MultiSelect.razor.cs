using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyMHWApp4.Pages.GeneralComponents.Select
{
    public partial class MultiSelect<T> : ComponentBase
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
        /// Bind to this value in case of multiple select.
        /// </summary>
        [Parameter]
        public List<T> ValueList { get; set; } = [];

        /// <summary>
        /// This Option applies an alternate style to the label
        /// </summary>
        [Parameter]
        public bool MutedLabel { get; set; } = false;

        /// <summary>
        /// Event callback for multiple value select changes.
        /// </summary>
        /// <value></value>
        [Parameter]
        public EventCallback<List<T>> ValueListChanged { get; set; }

        /// <summary>
        /// These options populate the select.
        /// </summary>
        [Parameter, EditorRequired]
        public required List<T> Options { get; set; } = [];

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

        private Guid id = Guid.NewGuid();

        private string os = "";

        private readonly Dictionary<string, object> selectAttributes = [];

        private string labelClass = string.Empty;

        private IJSObjectReference? module;

        protected override async Task OnInitializedAsync()
        {
            os = await JSRuntime.InvokeAsync<string>("getOperatingSystem", []);

            if (MutedLabel)
                labelClass = "muted";
            StateHasChanged();
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

        private void HandleNativeChange(ChangeEventArgs args)
        {
            if (null == args || null == args.Value)
                return;

            HandleNativeChangeMultiple(((string[])args.Value).Select(v => int.Parse(v)).ToArray());
        }

        private void HandleNativeChangeMultiple(int[] ids)
        {
            ValueList = Options.FindAll(o => ids.Contains((int)OptionValueMapper(o))).ToList();
            ValueListChanged.InvokeAsync(ValueList);
        }

        private void HandleNativeChangeMultiple(string[] ids)
        {
            ValueList = Options.FindAll(o => ids.Contains((string)OptionValueMapper(o))).ToList();
            ValueListChanged.InvokeAsync(ValueList);
        }

        private void HandleCustomChangeMultiple(T option)
        {
            if (ValueList.Contains(option))
            {
                ValueList.Remove(option);
            }
            else
            {
                ValueList.Add(option);
            }
            ValueListChanged.InvokeAsync(ValueList);
        }

        private bool IsSelected(T option)
        {
            var optionValue = OptionValueMapper(option);
            return ValueList.Any(v => OptionValueMapper(v)?.Equals(optionValue) == true);
        }
    }
}
