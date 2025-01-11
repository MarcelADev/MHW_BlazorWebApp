namespace MHWApp.Library.Model
{
    public class ParticipatorArmor
    {
        public int Id { get; set; }

        public int ParticipatorId { get; set; }
        public Participator Participator { get; set; } = null!;

        public int ArmorId { get; set; }
        public Armor Armor { get; set; } = null!;

        public int Count { get; set; }
    }
}
