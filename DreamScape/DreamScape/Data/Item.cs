using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Data
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } 
        public string Rarity { get; set; } 
        public int Power { get; set; }
        public int Speed { get; set; }
        public int Durability { get; set; }
        public string MagicProperties { get; set; }

        public ICollection<Inventory> Inventories { get; set; }
        public ICollection<TradeItem> TradeItems { get; set; }
    }
}
