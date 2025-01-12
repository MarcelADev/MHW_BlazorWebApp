namespace MHWApp.Library.Model
{
    public class Participator
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }
        public required Player Player { get; set; }

        public int SessionId { get; set; }
        public required Session Session { get; set; }

        public int WeaponTypeId { get; set; }
        public required WeaponType WeaponType { get; set; }

        public List<InventoryEntry> InventoryEntries { get; set; } = [];
        public List<ParticipatorInventoryEntry> ParticipatorInventoryEntries { get; set; } = [];

        public List<Weapon> Weapons { get; set; } = [];
        public List<ParticipatorWeapon> ParticipatorWeapons { get; set; } = [];

        public List<Armor> Armors { get; set; } = [];
        public List<ParticipatorArmor> ParticipatorArmors { get; set; } = [];
    }
}
