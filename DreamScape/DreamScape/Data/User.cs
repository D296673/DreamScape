using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } 
        public string Email { get; set; }
        public bool IsAdmin { get; set; }


        public ICollection<Inventory> Inventories { get; set; }
        public ICollection<Trade> SentTrades { get; set; }
        public ICollection<Trade> ReceivedTrades { get; set; }
    }
}
