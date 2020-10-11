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
        private readonly Verify _v;

        public CartController (DbT10Software _db, Verify v)
        {
            this._db = _db;
            _v = v;
        }
        public IActionResult Index()
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            List<Cart> carts = new List<Cart>();
            //validate session 
            if (_v.VerifySession(sessionId, _db))
            {
                ViewData["Logged"] = true;
                User user = _db.Sessions.FirstOrDefault(s => s.Id == sessionId).User;

                ViewData["Username"] = user.Username;

                //retrieve product number labeled beside icon
                carts = _db.Carts.Where(x => x.UserId == user.Id).ToList();
            }
            else
            {
                string cartCookie = HttpContext.Request.Cookies["guestCart"];
                //tentative cart; now user not log in;
                if (cartCookie != null)
                {
                    var guestCart = JsonSerializer.Deserialize<GuestCart>(HttpContext.Request.Cookies["guestCart"]);
                    carts = guestCart.Products;
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
            }

            if (carts.Count == 0)
            {
                ViewData["cart_quantity"] = 0;
                ViewData["Total"] = 0;
                return View("NoItemCart");
            } else
            {
                ViewData["cart_quantity"] = carts.Sum(cart => cart.Quantity);
                ViewData["ItemsInCart"] = carts;
                ViewData["Total"] = carts.Sum(cart => cart.Quantity * cart.Product.Price);
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
            return View(carts);

        }

        [HttpPost]
        public JsonResult Update(int productId, int quantity)
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            string newPrice;
            string totalPrice;

            if (_v.VerifySession(sessionId, _db))
            {
                User user = _db.Sessions.FirstOrDefault(session => session.Id == sessionId).User;
                Cart cart = _db.Carts.FirstOrDefault(cart => cart.UserId == user.Id && cart.ProductId == productId);

                cart.Quantity = quantity;

                _db.SaveChanges();

                newPrice = (cart.Quantity * cart.Product.Price).ToString();
                totalPrice = (_db.Carts.Where(cart => cart.UserId == user.Id).Sum(cart => cart.Quantity * cart.Product.Price)).ToString();

            } else
            {
                var guestCart = JsonSerializer.Deserialize<GuestCart>(HttpContext.Request.Cookies["guestCart"]);
                foreach (var product in guestCart.Products)
                {
                    if (product.ProductId == productId)
                    {
                        product.Quantity = quantity;
                    }
                }
                HttpContext.Response.Cookies.Append("guestCart", JsonSerializer.Serialize<GuestCart>(guestCart));

                newPrice = (_db.Products.FirstOrDefault(product => product.Id == productId).Price * quantity).ToString();
                totalPrice = (guestCart.Products.Sum(cart => cart.Quantity * cart.Product.Price)).ToString();
            }

            return Json(new
            {
                success = true,
                newPrice,
                totalPrice
            });
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
