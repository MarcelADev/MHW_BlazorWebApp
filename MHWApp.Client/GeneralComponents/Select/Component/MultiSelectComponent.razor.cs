using MHWApp.Client.GeneralComponents.Select.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MHWApp.Client.GeneralComponents.Select.Component
{
    public partial class MultiSelectComponent<T> : SelectComponentBase<T>
    {
        private FieldIdentifier _fieldIdentifier;
        private bool HasValidationError => EditContext is not null ? EditContext?.GetValidationMessages(_fieldIdentifier).Any() == true : false;

        [CascadingParameter]
        public EditContext? EditContext { get; set; }

        [Parameter]
        public Expression<Func<List<T>>>? ValueListExpression { get; set; }

        [Parameter]
        public List<string>? CssClass { get; set; }

        [Parameter]
        public List<T> ValueList { get; set; } = [];

        [Parameter]
        public EventCallback<List<T>> ValueListChanged { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (ValueListExpression is not null)
            {
                _fieldIdentifier = FieldIdentifier.Create(ValueListExpression);
            }
        }

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

        private string GetCSSClassesForMultiSelect()
        {
            if (EditContext is null || CssClass is null)
                return string.Empty;

            string cssClasses = string.Empty;

            if (HasValidationError)
                cssClasses += " invalid";

            else if (!EditContext.IsModified(_fieldIdentifier)) return cssClasses;

            else if (CssClass.Contains("valid"))
                cssClasses += " valid";

            return cssClasses;
        }
    }
}
