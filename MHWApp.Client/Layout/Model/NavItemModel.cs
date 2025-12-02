namespace MHWApp.Client.Layout.Model
{
    public class NavItemModel
    {
		private string _content;
        private List<NavItemModel>? _options;

        public string Content
		{
			get { return _content; }
			private set { _content = value; }
		}
		public List<NavItemModel>? Options
		{
			get { return _options; }
			private set { _options = value; }
		}

		public bool OnlyContent => _options == null;

        public NavItemModel(string content, List<NavItemModel>? options = null)
        {
			if (content == null)
			{
                throw new NavItemModelInvalidConstructorException();
            }

			this._content = content;

			if (options != null)
			{
				this._options = options;
			}
        }
    }

	public class NavItemModelInvalidConstructorException : Exception
	{
		public NavItemModelInvalidConstructorException() : base("NavItemModel has to have content.")
		{
		}
    }
}
