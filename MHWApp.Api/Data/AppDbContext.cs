using MHWApp.Library.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace MHWApp.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        DbSet<Player> Players { get; set; }
        DbSet<Session> Sessions { get; set; }
        DbSet<Participator> Participators { get; set; }

        DbSet<ParticipatorInventoryEntry> ParticipatorInventoryEntries { get; set; }
        DbSet<ParticipatorInventoryWeapon> ParticipatorInventoryWeapons { get; set; }
        DbSet<ParticipatorInventoryArmor> ParticipatorInventoryArmors { get; set; }

        DbSet<Category> Categories { get; set; }
        DbSet<InventoryEntryCategory> InventoryEntryCategories { get; set; }
        DbSet<WeaponCategory> WeaponCategories { get; set; }
        DbSet<ArmorCategory> ArmorCategories { get; set; }

        DbSet<InventoryEntry> InventoryEntries { get; set; }
        DbSet<Weapon> Weapons { get; set; }
        DbSet<Armor> Armors { get; set; }

        DbSet<WeaponBranch> WeaponBranches { get; set; }
        DbSet<WeaponType> WeaponTypes { get; set; }

        DbSet<WeaponCraftingIngredient> WeaponCraftingIngredients { get; set; }
        DbSet<ArmorCraftingIngredient> ArmorCraftingIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Participator
            modelBuilder.Entity<Participator>()
                .HasOne(o => o.Player)
                .WithMany()
                .HasForeignKey(o => o.PlayerId);

            modelBuilder.Entity<Participator>()
                .HasOne(o => o.Session)
                .WithMany()
                .HasForeignKey(o => o.SessionId);

            modelBuilder.Entity<Participator>()
                .HasOne(o => o.WeaponType)
                .WithMany()
                .HasForeignKey(o => o.WeaponTypeId);

            // LISTS HERE
            modelBuilder.Entity<Participator>()
                .HasMany(o => o.InventoryEntries)
                .WithMany()
                .UsingEntity<ParticipatorInventoryEntry>();
            modelBuilder.Entity<Participator>()
                .HasMany(o => o.Weapons)
                .WithMany()
                .UsingEntity<ParticipatorInventoryWeapon>();
            modelBuilder.Entity<Participator>()
                .HasMany(o => o.Armors)
                .WithMany()
                .UsingEntity<ParticipatorInventoryArmor>(
                "ParticipatorInventoryArmor",
                    (l => l.HasOne<Participator>()
                        .WithMany(e => e.ParticipatorInventoryArmors)
                        .HasForeignKey(e => e.ParticipatorId)),
                     (r => r
                         .HasOne<Armor>()
                         .WithMany(e => e.ParticipatorArmorEntries)
                         .HasForeignKey(e => e.ArmorId))
                );


            #endregion

            #region Inventory Entry Catagory & Weapon Category & Armor Catagory
            modelBuilder.Entity<InventoryEntryCategory>()
                .HasOne(o => o.Category)
                .WithOne()
                .HasForeignKey<InventoryEntryCategory>(o => o.CategoryId);

            modelBuilder.Entity<WeaponCategory>()
                .HasOne(o => o.Category)
                .WithOne()
                .HasForeignKey<WeaponCategory>(o => o.CategoryId);

            modelBuilder.Entity<ArmorCategory>()
                .HasOne(o => o.Category)
                .WithOne()
                .HasForeignKey<ArmorCategory>(o => o.CategoryId);
            #endregion

            #region Inventory Entry
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(o => o.InventoryCategory)
                .WithMany()
                .HasForeignKey(o => o.InventoryCategoryId);

            modelBuilder.Entity<InventoryEntry>()
                .HasOne(o => o.InventoryCategory)
                .WithMany()
                .HasForeignKey(o => o.InventoryCategoryId);
            #endregion

            #region Weapon Branch & Weapon
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(o => o.InventoryCategory)
                .WithMany()
                .HasForeignKey(o => o.InventoryCategoryId);
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
