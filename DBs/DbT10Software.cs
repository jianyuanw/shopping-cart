using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SA51_CA_Project_Team10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.DBs
{
    public class DbT10Software : DbContext
    {
        protected IConfiguration configuration;

        public DbT10Software(DbContextOptions<DbT10Software> options)
            : base(options) { }

        // public DbSet<ActivationCode> ActivationCodes { get; set; } // Tentatively commented out, can be deleted afterwards
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<User> Users { get; set; }

    }
}