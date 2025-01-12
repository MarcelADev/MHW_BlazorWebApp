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
        DbSet<ParticipatorWeapon> ParticipatorWeapons { get; set; }
        DbSet<ParticipatorArmor> ParticipatorArmors { get; set; }

        DbSet<Category> Categories { get; set; }
        DbSet<InventoryEntryCategory> InventoryEntryCategories { get; set; }
        DbSet<WeaponCategory> WeaponCategories { get; set; }
        DbSet<ArmorCategory> ArmorCategories { get; set; }

        DbSet<InventoryEntry> InventoryEntries { get; set; }
        DbSet<Weapon> Weapons { get; set; }
        DbSet<Armor> Armors { get; set; }

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

            // Many to Many Relationship of Participator and InventoryEntry
            modelBuilder.Entity<Participator>()
                .HasMany(o => o.InventoryEntries)
                .WithMany()
                .UsingEntity<ParticipatorInventoryEntry>(
                    "ParticipatorInventoryEntry",
                    l => l
                        .HasOne(i => i.InventoryEntry)
                        .WithMany(p => p.ParticipatorInventoryEntries)
                        .HasForeignKey(i => i.InventoryEntryId),
                    r => r
                        .HasOne(p => p.Participator)
                        .WithMany(p => p.ParticipatorInventoryEntries)
                        .HasForeignKey(p => p.ParticipatorId),
                    j =>
                    {
                        j.Property(o => o.Count).HasDefaultValue(1);
                    }
                );

            // Many to Many Relationship of Participator and Weapon
            modelBuilder.Entity<Participator>()
                .HasMany(o => o.Weapons)
                .WithMany()
                .UsingEntity<ParticipatorWeapon>(
                    "ParticipatorWeapon",
                    l => l
                        .HasOne(w => w.Weapon)
                        .WithMany(p => p.ParticipatorWeapons)
                        .HasForeignKey(w => w.WeaponId),
                    r => r
                        .HasOne(p => p.Participator)
                        .WithMany(p => p.ParticipatorWeapons)
                        .HasForeignKey(p => p.ParticipatorId),
                    j =>
                    {
                        j.Property(o => o.Count).HasDefaultValue(1);
                    }
                );

            // Many to Many Relationship of Participator and Armor
            modelBuilder.Entity<Participator>()
                .HasMany(o => o.Armors)
                .WithMany()
                .UsingEntity<ParticipatorArmor>(
                    "ParticipatorArmor",
                    l => l
                        .HasOne(e => e.Armor)
                        .WithMany(e => e.ParticipatorArmors)
                        .HasForeignKey(e => e.ArmorId),
                    r => r
                        .HasOne(e => e.Participator)
                        .WithMany(e => e.ParticipatorArmors)
                        .HasForeignKey(e => e.ParticipatorId),
                    j =>
                    {
                        j.Property(o => o.Count).HasDefaultValue(1);
                    }
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
                .HasOne(o => o.GameExtension)
                .WithMany()
                .HasForeignKey(o => o.GameExtensionId);

            // Many to Many Relationship of InventoryEntry and Participator
            modelBuilder.Entity<InventoryEntry>()
                .HasMany(o => o.Participators)
                .WithMany()
                .UsingEntity<ParticipatorInventoryEntry>(
                    "ParticipatorInventoryEntry",
                    l => l
                        .HasOne(w => w.Participator)
                        .WithMany(p => p.ParticipatorInventoryEntries)
                        .HasForeignKey(w => w.ParticipatorId),
                    r => r
                        .HasOne(p => p.InventoryEntry)
                        .WithMany(p => p.ParticipatorInventoryEntries)
                        .HasForeignKey(p => p.InventoryEntryId)
                );

            // Many to Many Relationship of InventoryEntry and Weapon
            modelBuilder.Entity<InventoryEntry>()
                .HasMany(o => o.Weapons)
                .WithMany()
                .UsingEntity<WeaponCraftingIngredient>(
                    "WeaponCraftingIngredient",
                    l => l
                        .HasOne(w => w.Weapon)
                        .WithMany(p => p.WeaponCraftingIngredients)
                        .HasForeignKey(w => w.WeaponId),
                    r => r
                        .HasOne(p => p.InventoryEntry)
                        .WithMany(p => p.WeaponCraftingIngredients)
                        .HasForeignKey(p => p.InventoryEntryId)
                );

            // Many to Many Relationship of InventoryEntry and Armor
            modelBuilder.Entity<InventoryEntry>()
                .HasMany(o => o.Armors)
                .WithMany()
                .UsingEntity<ArmorCraftingIngredient>(
                    "ArmorCraftingIngredient",
                    l => l
                        .HasOne(w => w.Armor)
                        .WithMany(p => p.ArmorCraftingIngredients)
                        .HasForeignKey(w => w.ArmorId),
                    r => r
                        .HasOne(p => p.InventoryEntry)
                        .WithMany(p => p.ArmorCraftingIngredients)
                        .HasForeignKey(p => p.InventoryEntryId)
                );
            #endregion

            #region Armor
            modelBuilder.Entity<Armor>()
                .HasOne(o => o.ArmorCategory)
                .WithMany()
                .HasForeignKey(o => o.ArmorCategoryId);

            modelBuilder.Entity<Armor>()
                .HasOne(o => o.GameExtension)
                .WithMany()
                .HasForeignKey(o => o.GameExtensionId);
            #endregion

            #region Weapon
            modelBuilder.Entity<Weapon>()
                .HasOne(o => o.WeaponCategory)
                .WithMany()
                .HasForeignKey(o => o.WeaponCategoryId);

            modelBuilder.Entity<Weapon>()
                .HasOne(o => o.WeaponType)
                .WithMany()
                .HasForeignKey(o => o.WeaponTypeId);

            modelBuilder.Entity<Weapon>()
                .HasOne(o => o.GameExtension)
                .WithMany()
                .HasForeignKey(o => o.GameExtensionId);

            modelBuilder.Entity<Weapon>()
                .HasOne(o => o.Predecessor)
                .WithMany()
                .HasForeignKey(o => o.PredecessorId);

            // Many to Many Relationship of Weapon and Participator
            modelBuilder.Entity<Weapon>()
                .HasMany(o => o.Participators)
                .WithMany()
                .UsingEntity<ParticipatorWeapon>(
                    "ParticipatorWeapon",
                    l => l
                        .HasOne(w => w.Participator)
                        .WithMany(p => p.ParticipatorWeapons)
                        .HasForeignKey(w => w.ParticipatorId),
                    r => r
                        .HasOne(p => p.Weapon)
                        .WithMany(p => p.ParticipatorWeapons)
                        .HasForeignKey(p => p.WeaponId)
                );

            // Many to Many Relationship of Weapon and InventoryEntry
            modelBuilder.Entity<Weapon>()
                .HasMany(o => o.InventoryEntries)
                .WithMany()
                .UsingEntity<WeaponCraftingIngredient>(
                    "WeaponCraftingIngredient",
                    l => l
                        .HasOne(w => w.InventoryEntry)
                        .WithMany(p => p.WeaponCraftingIngredients)
                        .HasForeignKey(w => w.InventoryEntryId),
                    r => r
                        .HasOne(p => p.Weapon)
                        .WithMany(p => p.WeaponCraftingIngredients)
                        .HasForeignKey(p => p.WeaponId)
                );
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
