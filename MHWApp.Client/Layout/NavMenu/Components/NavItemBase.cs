using MHWApp.Client.Layout.NavMenu.Model;
using Microsoft.AspNetCore.Components;

namespace MHWApp.Client.Layout.NavMenu.Components
{
    public abstract class NavItemBase : ComponentBase
    {
        [Parameter]
        public required NavItemBaseModel Model { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        protected string ID => Model.Title.ToLower().Replace(' ', '-');
    }
}