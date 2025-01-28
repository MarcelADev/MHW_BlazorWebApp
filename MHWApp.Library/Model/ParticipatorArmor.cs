namespace MHWApp.Library.Model
{
    public class ParticipatorArmor
    {
        public required int Id { get; set; }

        public required int ParticipatorId { get; set; }
        public Participator? Participator { get; set; }

        public required int ArmorId { get; set; }
        public Armor? Armor { get; set; }

        public required int Count { get; set; }
    }
}
