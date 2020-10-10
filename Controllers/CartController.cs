using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class CartController : Controller
    {
        private readonly DbT10Software _db;

        public CartController (DbT10Software _db)
        {
            this._db = _db;
        }
        public IActionResult Index(Verify v)
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            //validate session 
            if (v.VerifySession(sessionId, _db))
            {
                ViewData["Logged"] = true;
                User user = _db.Sessions.FirstOrDefault(s => s.Id == sessionId).User;

                ViewData["Username"] = user.Username;

                //retrieve product number labeled beside icon
                List<Cart> carts = _db.Carts.Where(x => x.UserId == user.Id).ToList();

                ViewData["cart_quantity"] = carts.Sum(cart => cart.Quantity);

                // pack Cart list and deliver to view; now user logged in;
                ViewData["ItemsInCart"] = carts;
            }
            else
            {
                string cartCookie = HttpContext.Request.Cookies["guestCart"];
                //tentative cart; now user not log in;
                if (cartCookie != null)
                {
                    var guestCart = JsonSerializer.Deserialize<GuestCart>(HttpContext.Request.Cookies["guestCart"]);

                    ViewData["cart_quantity"] = guestCart.Count();

                    ViewData["ItemsInCart"] = guestCart.Products;

                    // implement create List <object> and deliver to ViewData["ItemsInCart"]
                    // List<Cart> noLoginCart = new List<Cart>();

                    // get productId and merge all same product into List<Cart> noLoginCart
                    // packed static method in this class below: DeriveNoLoginCartListFromCookie
                    // noLoginCart = DeriveNoLoginCartListFromCookie(noLoginCart, cart);

                    // implement all information in List<Cart> noLoginCart
                    // packed static method in this class below: FillingAllInformationOfCartObjectBaseOnProductId
                    // noLoginCart = FillingAllInformationOfCartObjectBaseOnProductId(noLoginCart);

                    // ViewData["ItemsInCart"] = noLoginCart;
                }
                else
                {
                    ViewData["cart_quantity"] = 0;
                }
            }

            // check the cart_quantity, if no item in cart, return no item cart page;
            if((int)ViewData["cart_quantity"] == 0)
                return View("NoItemCart");

            //-----implement cart logic start here(please dont modify others part code)----//
            //implement...
            //implement...
            //-----implement cart logic end here(please dont modify others part code)----//

            //bold navbar 
            ViewData["Is_Cart"] = "font-weight: bold";
            return View("ItemCart");

        }

        /*public static List<Cart> DeriveNoLoginCartListFromCookie(List<Cart> noLoginCart, string[] cart)
        {
            for (int i = 0; i < cart.Length; i++)
            {
                if (cart[i] != "" && cart[i] != null)
                {
                    int currentCartItem = Convert.ToInt32(cart[i]);
                    for (int j = 0; j < noLoginCart.Count; j++)
                    {
                        if (currentCartItem != noLoginCart[j].ProductId)
                        {
                            noLoginCart.Add(new Cart { ProductId = currentCartItem, Quantity = 1 });
                        }
                        else if (currentCartItem == noLoginCart[j].ProductId)
                        {
                            noLoginCart[j].Quantity += 1;
                        }
                    }
                }
            }

            return noLoginCart;
        }

        public static List<Cart> FillingAllInformationOfCartObjectBaseOnProductId(List<Cart> noLoginCart)
        {
            for(int i = 0; i < noLoginCart.Count; i++)
            {
                int productIDinCode = noLoginCart[i].ProductId;
                noLoginCart[i].Product.Id = productIDinCode;
                // noLoginCart[i].Product = from Products where Id == productIDinCode select *;
            }
            return noLoginCart;
        }*/

    }
}
