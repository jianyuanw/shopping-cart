using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
    }
}
