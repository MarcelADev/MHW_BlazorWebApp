namespace MyMHWApp2.Data
{
    using Microsoft.EntityFrameworkCore;
    using MyMHWApp2.Components.Models;

    public class AppDbContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }

}
