using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Data
{
    public class TradeItem
    {
        public int Id { get; set; } 
        public int TradeId { get; set; } 
        public int ItemId { get; set; } 
        public int Quantity { get; set; } 
        public Trade Trade { get; set; }
        public Item Item { get; set; }
    }
}
