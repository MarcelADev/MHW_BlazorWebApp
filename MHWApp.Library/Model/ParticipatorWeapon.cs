namespace MHWApp.Library.Model
{
    public class ParticipatorWeapon
    {
        public required int Id { get; set; }

        public required int ParticipatorId { get; set; }
        public Participator? Participator { get; set; }

        public required int WeaponId { get; set; }
        public Weapon? Weapon { get; set; }

        public required int Count { get; set; }
    }
}
