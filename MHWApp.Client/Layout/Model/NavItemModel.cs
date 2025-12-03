namespace MHWApp.Client.Layout.Model
{
    public class NavItemModel
    {
		private string _title;
        private List<NavItemModel>? _options = [];

        public string Title
		{
			get { return _title; }
			private set { _title = value; }
		}
        public List<NavItemModel> Options
		{
			get { return _options!; }
			private set { _options = value; }
		}
		public bool HasOptions => Options.Any();
        public string? RedirectPath { get; private set; }
        public bool IsLink => RedirectPath != null;

        public NavItemModel(string title)
        {
            if (title == null || title == string.Empty)
            {
                throw new NavItemModelInvalidConstructorException();
            }

            this._title = title;
        }

        public NavItemModel(string title, List<NavItemModel> options) : this(title)
        {
            if (options == null)
            {
                throw new NavItemModelInvalidConstructorException();
            }

            this._options = options;
        }

        public NavItemModel(string title, string redirectPath) : this(title)
        {
            RedirectPath = redirectPath;
        }

        public NavItemModel(string title, List<NavItemModel> options, string redirectPath) : this(title, options)
        {
            RedirectPath = redirectPath;
        }
    }

	public class NavItemModelInvalidConstructorException : Exception
	{
		public NavItemModelInvalidConstructorException() : base("NavItemModel had invalid constructor parameters.")
		{
		}
    }
}
