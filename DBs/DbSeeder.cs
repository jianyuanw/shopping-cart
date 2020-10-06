using SA51_CA_Project_Team10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.DBs
{
    public class DbSeeder
    {
        private readonly DbT10Software _db;

        public DbSeeder(DbT10Software db)
        {
            _db = db;
        }
        public void Seed()
        {
            CreateUsers();
            CreateProducts();
            _db.SaveChanges();
        }

        private void CreateUsers()
        {
            string[] users = { "angelia", "derek", "jianyuan", "site", "yitong", "yubo", "zifeng" };
            foreach (string user in users)
            {
                string salt = GenerateSalt(50);
                _db.Add(new User
                {
                    Username = user,
                    Salt = salt,
                    Password = GenerateHashString(salt + user)
                });
            }
        }

        private void CreateProducts()
        {
            
            string[] names = { ".NET Charts", ".NET PayPal", ".NET ML", ".NET Analytics", ".NET Logger", ".NET Numerics" };
            string[] descriptions =
            {
                "Brings powerful charting capabilities to your .NET applications",
                "Integrate your .NET apps with PayPal the easy way!",
                "Supercharged .NET machine learning libraries.",
                "Performs data mining and analytics easily in .NET",
                "Logs and aggregates events easily in your .NET apps.",
                "Powerful numerical methods for your .NET simulations"
            };
            int[] prices = { 99, 69, 299, 299, 49, 199 };
            string[] imagelinks =
            {
                "NET_Charts.png",
                "NET_PayPal.png",
                "NET_ML",
                "NET_Analytics",
                "NET_Logger",
                "NET_Numerics"
            };

            for (int i = 0; i < names.Length; i++)
            {
                _db.Add(new Product
                {
                    Name = names[i],
                    Description = descriptions[i],
                    Price = prices[i],
                    ImageLink = "~/images/software/" + imagelinks[i]
                });
            }
        }

        private string GenerateHashString(string data)
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

        private string GenerateSalt(int length)
        {
            const string chars = "abcdefghiijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }        
    }
}
