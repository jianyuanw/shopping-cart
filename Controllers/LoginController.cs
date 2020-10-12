using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;
using Microsoft.AspNetCore.Http;

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

                Response.Cookies.Append("sessionId", guid, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax
                });

                TempData["Alert"] = "primary|Successfully logged in!";

                foreach (var cart in _db.Carts.Where(cart => cart.UserId == user.Id))
                {
                    _db.Carts.Remove(cart);
                }

                string cartCookie = HttpContext.Request.Cookies["guestCart"];                
                if (cartCookie != null)
                {
                    // Overwrites current cart in account with guestCart if guestCart exists as per CW's specifications
                    GuestCart guestCart = JsonSerializer.Deserialize<GuestCart>(cartCookie);
                    foreach (var product in guestCart.Products)
                    {
                        _db.Carts.Add(new Cart
                        {
                            ProductId = product.ProductId,
                            UserId = user.Id,
                            Quantity = product.Quantity
                        });
                    }
                    HttpContext.Response.Cookies.Delete("guestCart");
                    TempData["Alert"] += $" {guestCart.Count()} item(s) from your previous cart has overwritten your account cart.";
                }
                _db.SaveChanges();
            }
            
            // TempData was not expiring fast enough in some use cases, this ensures removal after single usage
            if (TempData["Redirect"] != null) 
            {
                string url = (string)TempData["Redirect"];
                TempData.Remove("Redirect");
                return Redirect(url);
            }
            return Redirect("/Gallery/Index");
        }
    }
}
