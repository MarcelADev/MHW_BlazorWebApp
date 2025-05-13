using MHWApp.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.DTO
{
    public class CategoryDTO
    {
        public required Category NewCategory { get; set; }
        public required bool IsInventoryEntry { get; set; }
        public required bool IsWeapon { get; set; }
        public required bool IsArmor { get; set; }
    }
}
