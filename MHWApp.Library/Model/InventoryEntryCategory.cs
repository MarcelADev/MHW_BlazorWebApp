namespace MHWApp.Library.Model
{
    public class InventoryEntryCategory
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
