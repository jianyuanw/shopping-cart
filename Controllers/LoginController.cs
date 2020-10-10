using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbT10Software _db;

        public LoginController(DbT10Software db)
        {
            _db = db;
        }
        public IActionResult Index(Verify v)
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            if (v.VerifySession(sessionId, _db))
            {
                TempData["Alert"] = "primary|Already logged in!";
                return Redirect("/Gallery/Index");
            }
            else {
                string cartCookie = HttpContext.Request.Cookies["guestCart"];
                if (cartCookie != null)
                {
                    GuestCart guestCart = JsonSerializer.Deserialize<GuestCart>(cartCookie);
                    ViewData["cart_quantity"] = guestCart.Count();
                }
                else
                {
                    ViewData["cart_quantity"] = 0;
                }
            }
            ViewData["Is_Login"] = "font-weight: bold";
            return View();
        }

        [HttpPost]
        public IActionResult Authenticate(Hasher hasher, string username, string password)
        {
            User user = _db.Users.FirstOrDefault(x => x.Username == username);
            if (user == null || hasher.GenerateHashString(password, user.Salt) != user.Password)
            {
                TempData["Alert"] = "danger|Username or password incorrect, please try again.";
                return Redirect("Index");                
            } else
            {
                // Create and store session
                string guid = Guid.NewGuid().ToString();
                _db.Sessions.Add(new Session
                {
                    Id = guid,
                    UserId = user.Id,
                    TimeStamp = DateTime.Now
                });
                _db.SaveChanges();
                Response.Cookies.Append("sessionId", guid);
                TempData["Alert"] = "primary|Successfully logged in!";

                string cartCookie = HttpContext.Request.Cookies["guestCart"];                
                if (cartCookie != null)
                {
                    // Merges guestCart with current cart in account if guestCart exists
                    GuestCart guestCart = JsonSerializer.Deserialize<GuestCart>(cartCookie);
                    foreach (var product in guestCart.Products)
                    {
                        var productInDb = _db.Carts.FirstOrDefault(cart => cart.ProductId == product.ProductId && cart.UserId == user.Id);
                        if (productInDb != null)
                        {
                            productInDb.Quantity += product.Quantity;
                        } else
                        {
                            _db.Carts.Add(new Cart
                            {
                                ProductId = product.ProductId,
                                UserId = user.Id,
                                Quantity = product.Quantity
                            });
                        }
                    }
                    _db.SaveChanges();
                    HttpContext.Response.Cookies.Delete("guestCart");
                    TempData["Alert"] += $" {guestCart.Count()} item(s) from your previous cart has been merged into your account cart.";
                }
            }
            
            if (TempData["Redirect"] != null) 
            {
                return Redirect((string) TempData["Redirect"]);
            }
            return Redirect("/Gallery/Index");
        }
    }
}
