using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class GalleryController : Controller
    {
        private readonly DbT10Software _db;
        private readonly Verify verify;

        public GalleryController(DbT10Software _db, Verify verify) {
            this._db = _db;
            this.verify = verify;
        }
        public IActionResult Index()
        {
            //alert message
            ViewData["Message"] = TempData["Message"];

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
            }

            //

            //retrieve products list and pass to view
            List<Product> products = _db.Products.ToList();
            ViewData["Products"] = products;

            //bold navbar 
            ViewData["Is_Gallery"] = "font-weight: bold";
            return View();
        }
    }
}
