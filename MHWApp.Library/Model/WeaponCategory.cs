namespace MHWApp.Library.Model
{
    public class WeaponCategory
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
