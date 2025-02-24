using DreamScape.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DreamScape.Login
{
    public static class LoginManager
    {
        private const string CookieFileName = "remember_token.txt";

        /// <summary>
        /// Logt een gebruiker in en maakt een veilige cookiebestand met een RememberToken.
        /// </summary>
        public static async Task<bool> LoginUserAsync(string username, string password)
        {
            using (var db = new AppDbContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null || !SecureHasher.Verify(password, user.Password))
                {
                    Debug.WriteLine("Ongeldige gebruikersnaam of wachtwoord.");
                    return false;
                }
                return true;

            }
        }

        
    }
}
