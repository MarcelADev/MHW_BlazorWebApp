namespace MHWApp.Library.Model
{
    public class ParticipatorWeapon
    {
        public int Id { get; set; }

        public int ParticipatorId { get; set; }
        public Participator Participator { get; set; }

        public int WeaponId { get; set; }
        public Weapon Weapon { get; set; }

        public int Count { get; set; }
    }
}
