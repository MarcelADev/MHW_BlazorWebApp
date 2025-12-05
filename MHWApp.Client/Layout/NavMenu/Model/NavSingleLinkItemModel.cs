namespace MHWApp.Client.Layout.NavMenu.Model
{
    public class NavSingleLinkItemModel : NavItemBaseModel
    {
        public NavSingleLinkItemModel(string title, string redirectPath) : base(title)
        {
            if (string.IsNullOrWhiteSpace(redirectPath))
                throw new NavItemModelInvalidConstructorException();

            base.RedirectPath = redirectPath;
        }
    }
}