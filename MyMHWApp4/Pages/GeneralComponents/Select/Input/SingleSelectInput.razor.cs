using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MyMHWApp4.Pages.GeneralComponents.Select.Input
{
    /// <summary>
    /// SingleSelectInput is specifically designed for use within a Editform. It uses the built in features of InputBase<T> to handle the value and validation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class SingleSelectInput<T> : SelectInputBase<T>
    {
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
            var selected = Options.Find(o => (int)OptionValueMapper(o) == id);
            if (!Equals(Value, selected))
                CurrentValue = selected!;
        }

        private void HandleNativeChangeSingle(string id)
        {
            var selected = Options.Find(o => (string)OptionValueMapper(o) == id);
            if (!Equals(Value, selected))
                CurrentValue = selected!;
        }

        private void HandleCustomChange(T option)
        {
            if (!Equals(Value, option))
                CurrentValue = option;
        }
    }
}
