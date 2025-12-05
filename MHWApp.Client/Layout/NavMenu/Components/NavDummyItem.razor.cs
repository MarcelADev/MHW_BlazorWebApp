using MHWApp.Client.Layout.NavMenu.Model;

namespace MHWApp.Client.Layout.NavMenu.Components
{
    public partial class NavDummyItem : NavItemBase
    {
        protected NavDummyItemModel ModelImplementation
        => (NavDummyItemModel)base.Model;
    }
}