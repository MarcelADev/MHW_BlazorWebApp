namespace MHWApp.Library.Model
{
    public class ParticipatorArmor : IModel
    {
        public int Id { get; set; }

        public int ParticipatorId { get; set; }
        public required Participator Participator { get; set; }

        public int ArmorId { get; set; }
        public required Armor Armor { get; set; }

        public int Count { get; set; }
    }
}
