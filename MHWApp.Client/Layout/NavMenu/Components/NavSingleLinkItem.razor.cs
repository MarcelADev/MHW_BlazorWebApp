using MHWApp.Client.Layout.NavMenu.Model;

namespace MHWApp.Client.Layout.NavMenu.Components
{
    public partial class NavSingleLinkItem : NavItemBase
    {
        protected NavSingleLinkItemModel ModelImplementation
        => (NavSingleLinkItemModel)base.Model;
    }
}