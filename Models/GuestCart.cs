using SA51_CA_Project_Team10.DBs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.Models
{
    public class GuestCart
    {
        public List<Cart> Products { get; set; }

        public GuestCart()
        {
            Products = new List<Cart>();
        }

        public void Add(int productId, Product product)
        {
            foreach (var item in Products)
            {
                if (item.ProductId == productId)
                {
                    item.Quantity++;
                    return;
                }
            }
            Products.Add(new Cart
            {
                Id = 0,
                ProductId = productId,
                Quantity = 1,
                UserId = 0
            });
        }

        public int Count()
        {
            int sum = 0;
            foreach (var item in Products)
            {
                sum += item.Quantity;
            }
            return sum;
        }

        public Cart Find(int productId)
        {
            foreach (var cart in Products)
            {
                if (cart.ProductId == productId)
                {
                    return cart;
                }
            }
            return null;
        }

        public List<Cart> LoadProducts(DbT10Software db)
        {
            List<Cart> loadedCart = new List<Cart>();
            foreach (var item in Products)
            {
                loadedCart.Add(new Cart
                {
                    Id = 0,
                    ProductId = item.ProductId,
                    Product = db.Products.FirstOrDefault(product => product.Id == item.ProductId),
                    Quantity = item.Quantity,
                    UserId = 0
                });
            }
            return loadedCart;
        }
    }
}
