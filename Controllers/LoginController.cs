using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbT10Software _db;
        private readonly Verify _verify;

        public LoginController(DbT10Software db, Verify v)
        {
            _db = db;
            _verify = v;
        }
        public IActionResult Index()
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            if (sessionId != null && _verify.VerifySession(sessionId, _db))
            {
                TempData["Message"] = "Already logged in!";
                return Redirect("/Gallery/Index");
            }
            ViewData["Is_Login"] = "font-weight: bold";
            return View();
        }

        [HttpPost]
        public IActionResult Authenticate(Hasher hasher, string username, string password)
        {
            User user = _db.Users.FirstOrDefault(x => x.Username == username);
            if (user == null || hasher.GenerateHashString(user.Salt + password) != user.Password)
            {
                TempData["Message"] = "Username or password incorrect, please try again.";
                return Redirect("Index");                
            } else {
                string cart = HttpContext.Request.Cookies["cart"];                
                if (cart == null)
                {
                    string guid = Guid.NewGuid().ToString();
                    _db.Sessions.Add(new Session
                    {
                        Id = guid,
                        UserId = user.Id,
                        TimeStamp = DateTime.Now
                    });
                    _db.SaveChanges();
                    Response.Cookies.Append("sessionId", guid);
                } else {
                    /* TODO: Cart merging logic here
                    if (sessionId != userSession.SessionId)
                    {
                        List<Cart> accountCart = _db.Carts.Where(cart => cart.SessionId == userSession.SessionId).ToList();

                        List<int> products = _db.Carts.Where(cart => cart.SessionId == userSession.SessionId)
                                                      .Select(cart => cart.ProductId).ToList();

                        List<Cart> currentCart = _db.Carts.Where(cart => cart.SessionId == sessionId).ToList();

                        foreach (Cart c in currentCart)
                        {
                            if (products.Contains(c.ProductId))
                            {
                                Cart 
                            }
                        }
                    }*/
                }
            }
            TempData["Message"] = "Successfully logged in!";
            if (TempData["Redirect"] != null) 
            {
                return Redirect((string) TempData["Redirect"]);
            }
            return Redirect("/Gallery/Index");
        }
    }
}
