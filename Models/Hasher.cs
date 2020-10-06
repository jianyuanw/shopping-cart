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
        public string GenerateHashString(string data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(data))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private byte[] GetHash(string data)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

    }
}
