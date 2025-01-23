namespace MHWApp.Library.Model
{
    public class ParticipatorWeapon : IModel
    {
        public int Id { get; set; }

        public int ParticipatorId { get; set; }
        public required Participator Participator { get; set; }

        public int WeaponId { get; set; }
        public required Weapon Weapon { get; set; }

        public int Count { get; set; }
    }
}
