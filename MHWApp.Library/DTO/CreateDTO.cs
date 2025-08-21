using MHWApp.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MHWApp.Library.StaticVariables;

namespace MHWApp.Library.DTO
{
    public class CreateDTO
    {
        public required string Name { get; set; }

        public Dictionary<ARMOR_TYPE, string>? ArmorSet { get; set; }

        public Dictionary<string, GameExtension>? InventoryEntries { get; set; }

    }
}
