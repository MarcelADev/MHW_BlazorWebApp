namespace MHWApp.Client.Layout.NavMenu.Model
{
    public abstract class NavItemBaseModel
    {
        public string Title { get; protected set; }
        public string? RedirectPath { get; protected set; } = null;
        protected NavItemBaseModel(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new NavItemModelInvalidConstructorException();

            Title = title;
        }
    }

    public class NavItemModelInvalidConstructorException : Exception
    {
        public NavItemModelInvalidConstructorException() : base("NavItemModel had invalid constructor parameters."){}
    }
}