using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        public IActionResult Index(int page)
        {

            //validate session
            if (verify.VerifySession(HttpContext.Request.Cookies["sessionId"], _db))
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
            else{  //tentative cart
                if (HttpContext.Request.Cookies["tempCart"] != null)
                {
                    String[] cart = HttpContext.Request.Cookies["tempCart"].Split("*");
                    int sum = 0;
                    foreach (string c in cart)
                        if (c != "" && c != null) ++sum;
                    ViewData["cart_quantity"] = sum;
                }
                else {
                    ViewData["cart_quantity"] = 0;
                }
            }

            string searchBar = HttpContext.Request.Cookies["searchbar"];
            List<Product> products = new List<Product>();

            //searchbar logic & retrieve products list and pass to view
            if (searchBar.IsNullOrEmpty())
            {
                products = _db.Products.OrderBy(x => x.Name).ToList();
            } else
            {
                products = _db.Products.Where(x => x.Name.Contains(searchBar)).OrderBy(x => x.Name).ToList();
            }                
            

            ViewData["Products"] = products;
            ViewData["Page"] = page;

            ViewData["searchbar"] = searchBar;

            //bold navbar 
            ViewData["Is_Gallery"] = "font-weight: bold";
            return View();
        }

        [HttpPost]
        public IActionResult ToPage(int toPage) {
            Debug.WriteLine(toPage);
            return RedirectToRoute(new { 
                controller = "Gallery",
                action = "Index",
                page = toPage
            });
        }

        [HttpPost]
        public IActionResult SearchAction(int toPage, string searchbar) {
            if (searchbar.IsNullOrEmpty()) {
                HttpContext.Response.Cookies.Delete("searchbar");
            }
            else {
                HttpContext.Response.Cookies.Append("searchbar", searchbar);
            }
            return RedirectToRoute(new
            {
                controller = "Gallery",
                action = "Index",
                page = toPage
            });
        }

        [HttpPost]
        public IActionResult AddCart(int productId) {
            if (verify.VerifySession(HttpContext.Request.Cookies["sessionId"], _db))
            {
                int userid = _db.Sessions.Where(x => x.Id == HttpContext.Request.Cookies["sessionId"]).ToList()[0].UserId;
                List<Cart> cart = _db.Carts.Where(x => x.UserId == userid && x.ProductId == productId).ToList();
                if (cart.Count == 0)
                {
                    _db.Add(new Cart()
                    {
                        Quantity = 1,
                        UserId = userid,
                        ProductId = productId
                    });
                }
                else
                {
                    cart[0].Quantity += 1;
                }
                _db.SaveChanges();

                return Json(new
                {
                    success = true
                });
            }
            else {
                if (HttpContext.Request.Cookies["tempCart"] != null)
                {
                    HttpContext.Response.Cookies.Append("tempCart", HttpContext.Request.Cookies["tempCart"] + "*" + productId);
                }
                else
                {
                    HttpContext.Response.Cookies.Append("tempCart", "*" + productId);
                }
                return Json(new
                {
                    success = true
                });
            }

            //return Json(new
            //{
                //success = false
           // });
        }
    }
}
