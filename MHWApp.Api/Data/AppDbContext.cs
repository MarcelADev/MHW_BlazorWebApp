using MHWApp.Library.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MHWApp.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : IdentityDbContext<Player, IdentityRole<int>, int>(options)
    {
        public required DbSet<Player> Players { get; set; }
        public required DbSet<Session> Sessions { get; set; }
        public required DbSet<Participator> Participators { get; set; }

        public required DbSet<ParticipatorInventoryEntry> ParticipatorInventoryEntries { get; set; }
        public required DbSet<ParticipatorWeapon> ParticipatorWeapons { get; set; }
        public required DbSet<ParticipatorArmor> ParticipatorArmors { get; set; }

        public required DbSet<Category> Categories { get; set; }
        public required DbSet<InventoryEntryCategory> InventoryEntryCategories { get; set; }
        public required DbSet<WeaponCategory> WeaponCategories { get; set; }
        public required DbSet<ArmorCategory> ArmorCategories { get; set; }
        public required DbSet<GameExtension> GameExtensions { get; set; }

        public required DbSet<InventoryEntry> InventoryEntries { get; set; }
        public required DbSet<Weapon> Weapons { get; set; }
        public required DbSet<Armor> Armors { get; set; }

        public required DbSet<WeaponType> WeaponTypes { get; set; }

        public required DbSet<WeaponCraftingIngredient> WeaponCraftingIngredients { get; set; }
        public required DbSet<ArmorCraftingIngredient> ArmorCraftingIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Participator
            modelBuilder.Entity<Participator>()
                .HasOne(p => p.Player)
                .WithMany()
                .HasForeignKey(p => p.PlayerId);

            modelBuilder.Entity<Participator>()
                .HasOne(p => p.Session)
                .WithMany()
                .HasForeignKey(p => p.SessionId);

            modelBuilder.Entity<Participator>()
                .HasOne(p => p.WeaponType)
                .WithMany()
                .HasForeignKey(p => p.WeaponTypeId);

            // Many to Many Relationship of Participator and InventoryEntry
            modelBuilder.Entity<Participator>()
                .HasMany(p => p.InventoryEntries)
                .WithMany(ie => ie.Participators)
                .UsingEntity<ParticipatorInventoryEntry>(
                    "ParticipatorInventoryEntry",
                    l => l
                        .HasOne(pie => pie.InventoryEntry)
                        .WithMany(ie => ie.ParticipatorInventoryEntries)
                        .HasForeignKey(pie => pie.InventoryEntryId),
                    r => r
                        .HasOne(pie => pie.Participator)
                        .WithMany(p => p.ParticipatorInventoryEntries)
                        .HasForeignKey(pie => pie.ParticipatorId),
                    j =>
                    {
                        j.Property(pie => pie.Count).HasDefaultValue(1);
                    }
                );

            // Many to Many Relationship of Participator and Weapon
            modelBuilder.Entity<Participator>()
                .HasMany(p => p.Weapons)
                .WithMany(w => w.Participators)
                .UsingEntity<ParticipatorWeapon>(
                    "ParticipatorWeapon",
                    l => l
                        .HasOne(pw => pw.Weapon)
                        .WithMany(w => w.ParticipatorWeapons)
                        .HasForeignKey(pw => pw.WeaponId),
                    r => r
                        .HasOne(pw => pw.Participator)
                        .WithMany(p => p.ParticipatorWeapons)
                        .HasForeignKey(pw => pw.ParticipatorId),
                    j =>
                    {
                        j.Property(pw => pw.Count).HasDefaultValue(1);
                    }
                );

            // Many to Many Relationship of Participator and Armor
            modelBuilder.Entity<Participator>()
                .HasMany(p => p.Armors)
                .WithMany(a => a.Participators)
                .UsingEntity<ParticipatorArmor>(
                    "ParticipatorArmor",
                    l => l
                        .HasOne(pa => pa.Armor)
                        .WithMany(a => a.ParticipatorArmors)
                        .HasForeignKey(pa => pa.ArmorId),
                    r => r
                        .HasOne(pa => pa.Participator)
                        .WithMany(p => p.ParticipatorArmors)
                        .HasForeignKey(pa => pa.ParticipatorId),
                    j =>
                    {
                        j.Property(pa => pa.Count).HasDefaultValue(1);
                    }
                );
            #endregion

            #region Category
            // one to one relationship of Category and InventoryEntryCategory
            modelBuilder.Entity<Category>()
                .HasOne(c => c.InventoryEntryCategory)
                .WithOne(iec => iec.Category)
                .HasForeignKey<InventoryEntryCategory>(iec => iec.CategoryId);

            // one to one relationship of Category and WeaponCategory
            modelBuilder.Entity<Category>()
                .HasOne(c => c.WeaponCategory)
                .WithOne(wc => wc.Category)
                .HasForeignKey<WeaponCategory>(wc => wc.CategoryId);

            // one to one relationship of Category and ArmorCategory
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ArmorCategory)
                .WithOne(ac => ac.Category)
                .HasForeignKey<ArmorCategory>(ac => ac.CategoryId);
            #endregion

            #region Inventory Entry Catagory & Weapon Category & Armor Catagory
            // one to one relationship of InventoryEntryCategory and Category
            modelBuilder.Entity<InventoryEntryCategory>()
                .HasOne(iec => iec.Category)
                .WithOne(c => c.InventoryEntryCategory)
                .HasForeignKey<InventoryEntryCategory>(iec => iec.CategoryId);

            // one to one relationship of WeaponCategory and Category
            modelBuilder.Entity<WeaponCategory>()
                .HasOne(wc => wc.Category)
                .WithOne(c => c.WeaponCategory)
                .HasForeignKey<WeaponCategory>(wc => wc.CategoryId);

            // one to one relationship of ArmorCategory and Category
            modelBuilder.Entity<ArmorCategory>()
                .HasOne(ac => ac.Category)
                .WithOne(c => c.ArmorCategory)
                .HasForeignKey<ArmorCategory>(ac => ac.CategoryId);
            #endregion

            #region Inventory Entry
            // many to one relationship of Inventory Entry and Game Extension
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(ie => ie.GameExtension)
                .WithMany()
                .HasForeignKey(ie => ie.GameExtensionId);

            // Many to Many Relationship of InventoryEntry and Participator
            modelBuilder.Entity<InventoryEntry>()
                .HasMany(ie => ie.Participators)
                .WithMany(p => p.InventoryEntries)
                .UsingEntity<ParticipatorInventoryEntry>(
                    "ParticipatorInventoryEntry",
                    l => l
                        .HasOne(pie => pie.Participator)
                        .WithMany(p => p.ParticipatorInventoryEntries)
                        .HasForeignKey(pie => pie.ParticipatorId),
                    r => r
                        .HasOne(pie => pie.InventoryEntry)
                        .WithMany(ie => ie.ParticipatorInventoryEntries)
                        .HasForeignKey(pie => pie.InventoryEntryId)
                );

            // Many to Many Relationship of InventoryEntry and Weapon
            modelBuilder.Entity<InventoryEntry>()
                .HasMany(ie => ie.Weapons)
                .WithMany(w => w.InventoryEntries)
                .UsingEntity<WeaponCraftingIngredient>(
                    "WeaponCraftingIngredient",
                    l => l
                        .HasOne(wci => wci.Weapon)
                        .WithMany(w => w.WeaponCraftingIngredients)
                        .HasForeignKey(w => w.WeaponId),
                    r => r
                        .HasOne(wci => wci.InventoryEntry)
                        .WithMany(ie => ie.WeaponCraftingIngredients)
                        .HasForeignKey(wci => wci.InventoryEntryId)
                );

            // Many to Many Relationship of InventoryEntry and Armor
            modelBuilder.Entity<InventoryEntry>()
                .HasMany(ie => ie.Armors)
                .WithMany(a => a.InventoryEntries)
                .UsingEntity<ArmorCraftingIngredient>(
                    "ArmorCraftingIngredient",
                    l => l
                        .HasOne(aci => aci.Armor)
                        .WithMany(a => a.ArmorCraftingIngredients)
                        .HasForeignKey(aci => aci.ArmorId),
                    r => r
                        .HasOne(aci => aci.InventoryEntry)
                        .WithMany(ie => ie.ArmorCraftingIngredients)
                        .HasForeignKey(aci => aci.InventoryEntryId)
                );

            // one to many relationship of InventoryEntry and InventoryEntryCategory
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(ie => ie.NamePrefix)
                .WithMany() // no reverse navigation from InventoryEntryCategory
                .HasForeignKey(ie => ie.NamePrefixId)
                .OnDelete(DeleteBehavior.SetNull); // ← set NamePrefixId = null when category is deleted
            #endregion

            #region Armor
            // many to one relationship of Armor and Game Extension
            modelBuilder.Entity<Armor>()
                .HasOne(a => a.GameExtension)
                .WithMany()
                .HasForeignKey(a => a.GameExtensionId);

            // One to Many Relationship of Armor and ArmorCategory
            modelBuilder.Entity<Armor>()
                .HasOne(a => a.ArmorCategory)
                .WithMany()
                .HasForeignKey(a => a.ArmorCategoryId);

            // Many to Many Relationship of Armor and Participator
            modelBuilder.Entity<Armor>()
                .HasMany(a => a.Participators)
                .WithMany(p => p.Armors)
                .UsingEntity<ParticipatorArmor>(
                    "ParticipatorArmor",
                    r => r
                        .HasOne(pa => pa.Participator)
                        .WithMany(p => p.ParticipatorArmors)
                        .HasForeignKey(pa => pa.ParticipatorId),
                    l => l
                        .HasOne(pa => pa.Armor)
                        .WithMany(a => a.ParticipatorArmors)
                        .HasForeignKey(pa => pa.ArmorId)
                );

            // Many to Many Relationship of Armor and InventoryEntry
            modelBuilder.Entity<Armor>()
                .HasMany(a => a.InventoryEntries)
                .WithMany(ie => ie.Armors)
                .UsingEntity<ArmorCraftingIngredient>(
                    "ArmorCraftingIngredient",
                    l => l
                        .HasOne(aci => aci.InventoryEntry)
                        .WithMany(ie => ie.ArmorCraftingIngredients)
                        .HasForeignKey(aci => aci.InventoryEntryId),
                    r => r
                        .HasOne(aci => aci.Armor)
                        .WithMany(a => a.ArmorCraftingIngredients)
                        .HasForeignKey(aci => aci.ArmorId)
                );
            #endregion

            #region Weapon
            // many to one relationship of Inventory Entry and Game Extension
            modelBuilder.Entity<Weapon>()
                .HasOne(w => w.GameExtension)
                .WithMany()
                .HasForeignKey(w => w.GameExtensionId);

            modelBuilder.Entity<Weapon>()
                .HasOne(w => w.WeaponCategory)
                .WithMany()
                .HasForeignKey(w => w.WeaponCategoryId);

            modelBuilder.Entity<Weapon>()
                .HasOne(w => w.WeaponType)
                .WithMany()
                .HasForeignKey(w => w.WeaponTypeId);

            modelBuilder.Entity<Weapon>()
                .HasOne(w => w.Predecessor)
                .WithMany()
                .HasForeignKey(w => w.PredecessorId);

            // Many to Many Relationship of Weapon and Participator
            modelBuilder.Entity<Weapon>()
                .HasMany(w => w.Participators)
                .WithMany(p => p.Weapons)
                .UsingEntity<ParticipatorWeapon>(
                    "ParticipatorWeapon",
                    l => l
                        .HasOne(pw => pw.Participator)
                        .WithMany(p => p.ParticipatorWeapons)
                        .HasForeignKey(pw => pw.ParticipatorId),
                    r => r
                        .HasOne(pw => pw.Weapon)
                        .WithMany(w => w.ParticipatorWeapons)
                        .HasForeignKey(pw => pw.WeaponId)
                );

            // Many to Many Relationship of Weapon and InventoryEntry
            modelBuilder.Entity<Weapon>()
                .HasMany(w => w.InventoryEntries)
                .WithMany(ie => ie.Weapons)
                .UsingEntity<WeaponCraftingIngredient>(
                    "WeaponCraftingIngredient",
                    l => l
                        .HasOne(wci => wci.InventoryEntry)
                        .WithMany(ie => ie.WeaponCraftingIngredients)
                        .HasForeignKey(wci => wci.InventoryEntryId),
                    r => r
                        .HasOne(wci => wci.Weapon)
                        .WithMany(w => w.WeaponCraftingIngredients)
                        .HasForeignKey(wci => wci.WeaponId)
                );
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
