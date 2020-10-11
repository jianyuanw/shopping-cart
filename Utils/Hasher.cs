using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SA51_CA_Project_Team10.DBs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.Models
{
    public class Hasher
    {        
        public string GenerateHashString(string password, string saltString)
        {
            byte[] salt = Convert.FromBase64String(saltString);
            byte[] encrypted = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, 32);
            return Convert.ToBase64String(encrypted);
                
        }
        public string GenerateActivationKey(DbT10Software db)
        {
            Random r = new Random();
            const string chars = "abcdefghiijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            List<string> keys = db.OrderDetails.Select(od => od.Id).ToList();

            StringBuilder sb = null;
            while (sb == null || keys.Contains(sb.ToString()))
            {
                sb = new StringBuilder();
                for (int i = 0; i < 14; i++)
                {
                    if (i == 4 || i == 9) { sb.Append("-"); }
                    else { sb.Append(chars[r.Next(chars.Length)]); }
                }
            }

            return sb.ToString();
        }
    }
}
