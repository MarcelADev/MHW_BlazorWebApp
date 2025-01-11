using System.ComponentModel.DataAnnotations;

namespace MyMHWApp.Components.Models
{
    public class InventoryEntry
    {
        [Key]
        public int Id { get; set; }
        public string EntryName { get; set; }
        public int Counter { get; set; }
        public int InventoryCategoryID { get; set; }
    }
}
