using MHWApp.Client.Layout.NavMenu.Components;
using MHWApp.Client.Layout.NavMenu.Model;

namespace MHWApp.Client.Layout.NavMenu.Components
{
    public partial class NavDropdownItem : NavItemBase
    {
        private bool isOpen = false;

        protected NavDropdownItemModel ModelImplementation
        => (NavDropdownItemModel)base.Model;

        private string NavDropDownID => ID + "-nav-dropdown";
        private string NavDropDownMenuID => ID + "-nav-dropdown-menu";
    }
}