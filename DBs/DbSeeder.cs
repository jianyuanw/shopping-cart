using Microsoft.AspNetCore.Authorization.Infrastructure;
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
            CreateOrders(10);
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
            _db.SaveChanges();
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
            _db.SaveChanges();
        }

        private void CreateOrders(int orders)
        {
            List<User> users = _db.Users.ToList();
            List<Product> products = _db.Products.ToList();
            Random r = new Random();
            

            for (int i = 0; i < orders; i++)
            {
                List<int> used = new List<int>();
                var startDate = new DateTime(2010, 1, 1);
                var randomUser = users[r.Next(users.Count)];
                var randomDate = startDate.AddDays(r.Next((DateTime.Today - startDate).Days));
                var randomOrder = new Order { UserId = randomUser.Id, DateTime = randomDate };
                _db.Orders.Add(randomOrder);
                _db.SaveChanges();

                for (int j = 0; j < r.Next(1, 6); j++)
                {
                    var randomNumber = r.Next(products.Count);
                    while (used.Contains(randomNumber))
                    {
                        randomNumber = r.Next(products.Count);
                    }
                    used.Add(randomNumber);
                    var randomProduct = products[randomNumber];
                    var randomQuantity = r.Next(1, 6);
                    var randomItem = new OrderDetail { OrderId = randomOrder.Id, ProductId = randomProduct.Id, Quantity = randomQuantity };
                    _db.OrderDetails.Add(randomItem);
                }
                _db.SaveChanges();
            }
        }

        private DateTime RandomDate()
        {
            Random gen = new Random();
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            
            return start.AddDays(gen.Next(range));
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
