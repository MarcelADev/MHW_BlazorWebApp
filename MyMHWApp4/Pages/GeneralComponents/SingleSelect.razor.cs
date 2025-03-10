using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMHWApp4.Pages.GeneralComponents
{
    public partial class SingleSelect<T> : ComponentBase
    {
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

        private Guid id = Guid.NewGuid();

        private string os = "";

        private Dictionary<string, object> selectAttributes = [];

        protected override async Task OnInitializedAsync()
        {
            os = await JSRuntime.InvokeAsync<string>("getOperatingSystem", []);
        }

        protected override void OnParametersSet()
        {
            _optionType = OptionValueMapper(Options[0]).GetType();
        }

        private void HandleNativeChange(ChangeEventArgs args)
        {
            if (null == args || null == args.Value) return;
            
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
            Value = option;
            ValueChanged.InvokeAsync(Value);
        }

        private bool IsSelected(T option)
        {
            return Value!.Equals(option);
        }
    }
}