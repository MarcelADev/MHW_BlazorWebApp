namespace MHWApp.Library.Model
{
    public class ParticipatorInventoryArmor
    {
        public int Id { get; set; }

        public int ParticipatorId { get; set; }
        public Participator Participator { get; set; }

        public int ArmorId { get; set; }
        public Armor Armor { get; set; }

        public int Count { get; set; }
    }
}
