namespace MHWApp.Library.Model
{
    public class Participator
    {
        public required int Id { get; set; }

        public required int PlayerId { get; set; }
        public Player? Player { get; set; }

        public required int SessionId { get; set; }
        public Session? Session { get; set; }

        public required int WeaponTypeId { get; set; }
        public WeaponType? WeaponType { get; set; }

        public List<InventoryEntry> InventoryEntries { get; set; } = [];
        public List<ParticipatorInventoryEntry> ParticipatorInventoryEntries { get; set; } = [];

        public List<Weapon> Weapons { get; set; } = [];
        public List<ParticipatorWeapon> ParticipatorWeapons { get; set; } = [];

        public List<Armor> Armors { get; set; } = [];
        public List<ParticipatorArmor> ParticipatorArmors { get; set; } = [];

        /// <summary>
        /// Clones the Participator with only the required properties
        /// </summary>
        /// <returns></returns>
        public Participator Clone()
        {
            return new Participator
            {
                Id = 0,
                PlayerId = this.PlayerId,
                SessionId = this.SessionId,
                WeaponTypeId = this.WeaponTypeId,
            };
        }
    }
}
