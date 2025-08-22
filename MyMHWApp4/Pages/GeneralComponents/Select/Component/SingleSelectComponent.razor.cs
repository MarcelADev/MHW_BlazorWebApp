using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MyMHWApp4.Pages.GeneralComponents.Select.Component
{
    public partial class SingleSelectComponent<T> : SelectComponentBase<T>
    {
        [Parameter]
        public T? Value { get; set; }

        [Parameter]
        public EventCallback<T?> ValueChanged { get; set; }

        private Type? _optionType;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (base.Options.Count > 0)
                _optionType = base.OptionValueMapper(base.Options[0]).GetType();
        }

        private void HandleNativeChange(ChangeEventArgs args)
        {
            if (args?.Value == null) return;

            if (_optionType == typeof(int))
                HandleNativeChangeSingle(int.Parse((string)args.Value));
            else if (_optionType == typeof(string))
                HandleNativeChangeSingle((string)args.Value);
        }

        private void HandleNativeChangeSingle(int id)
        {
            Value = base.Options.Find(o => (int)OptionValueMapper(o) == id);
            ValueChanged.InvokeAsync(Value);
        }

        private void HandleNativeChangeSingle(string id)
        {
            Value = base.Options.Find(o => (string)OptionValueMapper(o) == id);
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
    }
}
