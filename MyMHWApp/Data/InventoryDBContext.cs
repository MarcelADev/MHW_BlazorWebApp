using Microsoft.EntityFrameworkCore;
using MyMHWApp.Components.Models;

namespace MyMHWApp.Data
{
    public class InventoryDBContext : DbContext
    {
        public InventoryDBContext(DbContextOptions<InventoryDBContext> options) : base(options) { }

        public DbSet<InventoryEntry> InventoryEntries { get; set; }
    }
}