using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class CartController : Controller
    {
        private readonly DbT10Software _db;
        private readonly Verify verify;

        public CartController (DbT10Software _db, Verify verify)
        {
            this._db = _db;
            this.verify = verify;
        }
        public IActionResult Index()
        {

            //validate session 
            if (HttpContext.Request.Cookies["sessionId"] != null && verify.VerifySession(HttpContext.Request.Cookies["sessionId"], _db))
            {
                ViewData["Logged"] = true;
                int userId = _db.Sessions.Where(x => x.Id == HttpContext.Request.Cookies["sessionId"]).ToList()[0].UserId;
                ViewData["Username"] = _db.Users.Where(x => x.Id == userId).ToList()[0].Username;

                //retrieve product number labeled beside icon
                List<Cart> carts = _db.Carts.Where(x => x.UserId == userId).ToList();
                int total = 0;
                foreach (Cart cart in carts)
                    total += cart.Quantity;
                ViewData["cart_quantity"] = total;

                // pack Cart list and deliver to view
                ViewData["ItemsInCart"] = carts;
            }
            else
            {  //tentative cart
                if (HttpContext.Request.Cookies["tempCart"] != null)
                {
                    String[] cart = HttpContext.Request.Cookies["tempCart"].Split("*");
                    int sum = 0;
                    foreach (string c in cart)
                        if (c != "" && c != null) ++sum;
                    ViewData["cart_quantity"] = sum;

                    // -----implement create List <object> and deliver to ViewData["ItemsInCart"]
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

    }
}
