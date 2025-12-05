namespace MHWApp.Client.Layout.NavMenu.Model
{
    public class NavDropdownItemModel : NavItemBaseModel
    {
        public List<NavItemBaseModel> Options { get; private set; }
		public bool HasOptions => Options.Any();
        public bool IsLink => RedirectPath != null;

        public NavDropdownItemModel(string title, List<NavItemBaseModel> options) : base(title)
        {
            if (options == null)
            {
                throw new NavItemModelInvalidConstructorException();
            }

            Options = options;
        }

        public NavDropdownItemModel(string title, List<NavItemBaseModel> options, string redirectPath) : this(title, options)
        {
            if (string.IsNullOrWhiteSpace(redirectPath))
                throw new NavItemModelInvalidConstructorException();

            base.RedirectPath = redirectPath;
        }
    }
}
