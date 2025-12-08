namespace MHWApp.Client.Layout.NavMenu.Model
{
    public class NavDummyItemModel : NavItemBaseModel
    {
        public NavDummyItemModel(string title) : base(title) {}
        public NavDummyItemModel(string title, string description) : this(title) 
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new NavItemModelInvalidConstructorException();
            }

            base.Description = description;
        }
    }
}