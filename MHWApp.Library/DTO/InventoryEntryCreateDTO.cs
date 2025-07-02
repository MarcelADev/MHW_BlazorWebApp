using MHWApp.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.DTO
{
    public class InventoryEntryCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public List<int> InventoryEntryCategoryIds { get; set; } = [];

        public int Order { get; set; }
        public int? NamePrefixId { get; set; }
    }
}
