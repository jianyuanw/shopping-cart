using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly DbT10Software _db;

        public PurchaseController(DbT10Software db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // Retrieve session
            Session session = _db.Sessions.FirstOrDefault(x => x.Id == Request.Cookies["sessionId"]);

            // If session exists, means user is logged in.
            if (session != null)
            {
                // Logout button
                ViewData["Logged"] = true;

                User user = _db.Users.FirstOrDefault(x => x.Id == session.UserId);

                // Display username at top left
                ViewData["Username"] = user.Username;

                // Retrieve OrderDetails, Orders, Products from DB
                List<OrderDetail> orderDetails = _db.OrderDetails.ToList();
                List<Order> orders = _db.Orders.ToList();
                List<Product> products = _db.Products.ToList();

                // Pass to View
                ViewData["orderDetails"] = orderDetails;
                ViewData["orders"] = orders;
                ViewData["products"] = products;
                ViewData["user"] = user;

                // Bold menu item
                ViewData["Is_Purchase"] = "font-weight: bold";

                return View();
            }
            else // Else user is not logged in. Redirect to login page.
            {
                TempData["Alert"] = "primary|Please log in to view your purchases.";

                return RedirectToAction("Index", "Login");
            }
        }
    }
}
