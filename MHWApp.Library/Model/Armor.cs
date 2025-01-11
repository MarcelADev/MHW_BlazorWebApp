namespace MHWApp.Library.Model
{
    public class Armor
    {
        public int Id { get; set; }

        public int ArmorCategoryId { get; set; }
        public ArmorCategory ArmorCategory { get; set; }

        public int GameExtensionId { get; set; }
        public GameExtension GameExtension { get; set; }

        public string Name { get; set; }

        public List<ParticipatorArmorEntry> ParticipatorArmorEntries { get; set; } = [];
    }
}
