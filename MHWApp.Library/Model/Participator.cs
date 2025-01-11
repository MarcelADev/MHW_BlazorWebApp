using System.ComponentModel.DataAnnotations.Schema;

namespace MHWApp.Library.Model
{
    public class Participator
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }

        public int WeaponTypeId { get; set; }
        public WeaponType WeaponType { get; set; }

        public List<InventoryEntry> InventoryEntries { get; set; } = [];
        public List<Weapon> Weapons { get; set; } = [];
        public List<Armor> Armors { get; set; } = [];


        public List<ParticipatorInventoryEntry> ParticipatorEntries { get; set; } = [];
        public List<ParticipatorInventoryWeapon> ParticipatorWeapons { get; set; } = [];
        public List<ParticipatorArmor> ParticipatorArmors { get; set; } = [];
    }
}
