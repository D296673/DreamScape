using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Windows.System;

namespace DreamScape.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradeItem> TradeItems { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
            "server=localhost;user=root;password=;database=DreamScape",
            ServerVersion.Parse("8.0.30")

            );
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trade>()
             .HasOne(t => t.Receiver)
             .WithMany(u => u.ReceivedTrades) 
             .HasForeignKey(t => t.ReceiverId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "ShadowSlayer", Password = SecureHasher.Hash("Test123!"), Role = "Speler", Email = "shadow@example.com", IsAdmin = false },
                new User { Id = 2, Username = "MysticMage", Password = SecureHasher.Hash("Mage2024"), Role = "Speler", Email = "mystic@example.com", IsAdmin = false },
                new User { Id = 3, Username = "DragonKnight", Password = SecureHasher.Hash("Dragon!99"), Role = "Speler", Email = "dragon@example.com", IsAdmin = false },
                new User { Id = 4, Username = "AdminMaster", Password = SecureHasher.Hash("Admin007"), Role = "Beheerder", Email = "admin@example.com", IsAdmin= true },
                new User { Id = 5, Username = "ThunderRogue", Password = SecureHasher.Hash("Thund3r!!"), Role = "Speler", Email = "thunder@example.com", IsAdmin= false }
            );

            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 101, Name = "Zwaard des Vuur", Description = "Een mythisch zwaard met een vlammende gloed.", Type = "Wapen", Rarity = "Legendarisch", Power = 90, Speed = 60, Durability = 80, MagicProperties = "+30% vuurschade" },
                new Item { Id = 102, Name = "IJs Amulet", Description = "Een amulet dat de drager beschermt tegen kou.", Type = "Accessoire", Rarity = "Episch", Power = 20, Speed = 10, Durability = 70, MagicProperties = "+25% weerstand tegen ijsaanvallen" },
                new Item { Id = 103, Name = "Schaduw Mantel", Description = "Een donkere mantel die je bewegingen verbergt.", Type = "Armor", Rarity = "Zeldzaam", Power = 40, Speed = 85, Durability = 50, MagicProperties = "+15% kans om aanvallen te ontwijken" },
                new Item { Id = 104, Name = "Hamer der Titanen", Description = "Een massieve hamer met de kracht van de aarde.", Type = "Wapen", Rarity = "Legendarisch", Power = 95, Speed = 40, Durability = 90, MagicProperties = "Kan vijanden 3 sec verdoven" },
                new Item { Id = 105, Name = "Lichtboog", Description = "Een boog die pijlen van pure energie afvuurt.", Type = "Wapen", Rarity = "Episch", Power = 85, Speed = 75, Durability = 60, MagicProperties = "+10% kans op kritieke schade" },
                new Item { Id = 106, Name = "Helende Ring", Description = "Een ring die de gezondheid van de drager herstelt.", Type = "Accessoire", Rarity = "Zeldzaam", Power = 10, Speed = 5, Durability = 100, MagicProperties = "+5 HP per seconde" },
                new Item { Id = 107, Name = "Demonen Harnas", Description = "Een verdoemd harnas met duistere krachten.", Type = "Armor", Rarity = "Legendarisch", Power = 75, Speed = 50, Durability = 95, MagicProperties = "Absorbeert 20% van ontvangen schade" }
            );

            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { Id = 1, UserId = 1, ItemId = 101, Quantity = 1 },
                new Inventory { Id = 2, UserId = 2, ItemId = 102, Quantity = 1 },
                new Inventory { Id = 3, UserId = 3, ItemId = 103, Quantity = 1 },
                new Inventory { Id = 4, UserId = 4, ItemId = 104, Quantity = 1 },
                new Inventory { Id = 5, UserId = 5, ItemId = 105, Quantity = 1 }
            );
        }
    }
}



