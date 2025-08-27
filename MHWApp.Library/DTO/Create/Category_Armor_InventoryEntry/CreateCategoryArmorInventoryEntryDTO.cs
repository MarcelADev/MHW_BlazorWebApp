using MHWApp.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MHWApp.Library.StaticVariables;

namespace MHWApp.Library.DTO.Create.Category_Armor_InventoryEntry
{
    public class CreateCategoryArmorInventoryEntryDTO
    {
        public required string Name { get; set; }

        public int? GeneralGameExtensionId { get; set; }

        public List<ArmorModel>? ArmorSet { get; set; }

        public List<InventoryEntryModel>? InventoryEntries { get; set; }

    }

    public class ArmorModel
    {
        public required string Name { get; set; }
        public required ArmorType ArmorType { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is ArmorModel armor)
            {
                return armor.Name == Name && armor.ArmorType == ArmorType;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ArmorType);
        }
    }

    public class InventoryEntryModel
    {
        public required string Name { get; set; }
        public required GameExtension GameExtension { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is InventoryEntryModel inventoryEntry)
            {
                return inventoryEntry.Name == Name && inventoryEntry.GameExtension == GameExtension;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, GameExtension);
        }
    }
}
