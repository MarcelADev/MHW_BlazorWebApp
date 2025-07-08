using MHWApp.Library.Model;

namespace MyMHWApp4.Pages.Admin.Create
{
    public class InventoryEntryCardModel
    {
        public string Name { get; set; } = string.Empty;

        public GameExtension? GameExtension { get; set; }
    }
}
