using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHWApp.Client.Pages.GeneralComponents.Select.Component
{
    public partial class MultiSelectComponent<T> : SelectComponentBase<T>
    {
        [Parameter]
        public List<T> ValueList { get; set; } = [];

        [Parameter]
        public EventCallback<List<T>> ValueListChanged { get; set; }

        private void HandleNativeChange(ChangeEventArgs args)
        {
            if (args?.Value == null) return;

            if (args.Value is string[] sArray)
                HandleNativeChangeMultiple(sArray.Select(v => int.TryParse(v, out var i) ? i : (object)v).ToArray());
        }

        private void HandleNativeChangeMultiple(object[] ids)
        {
            var list = base.Options.Where(o => ids.Contains(base.OptionValueMapper(o))).ToList();
            ValueList = list;
            ValueListChanged.InvokeAsync(ValueList);
        }

        private void HandleCustomChangeMultiple(T option)
        {
            if (ValueList.Contains(option))
                ValueList.Remove(option);
            else
                ValueList.Add(option);

            ValueListChanged.InvokeAsync(ValueList);
        }
    }
}
