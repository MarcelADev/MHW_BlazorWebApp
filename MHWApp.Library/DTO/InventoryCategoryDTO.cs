using MHWApp.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.DTO
{
    public class InventoryCategoryDTO
    {
        public required InventoryEntryCategory InventoryEntryCategory { get; set; }
        public required Category Category { get; set; }
    }
}
