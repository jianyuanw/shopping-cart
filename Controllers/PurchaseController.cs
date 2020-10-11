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

        public IActionResult Index(Verify v)
        {
            // Retrieve session
            string sessionId = Request.Cookies["sessionId"];

            if (v.VerifySession(sessionId, _db)) // If session exists, means user is logged in.
            {
                // Logout button
                ViewData["Logged"] = true;

                // Bold menu item
                ViewData["Is_Purchase"] = "font-weight: bold";

                User user = _db.Sessions.FirstOrDefault(x => x.Id == sessionId).User;

                // Display username at top left
                ViewData["Username"] = user.Username;

                // Retrieve OrderDetails from DB
                List<OrderDetail> orderDetail = _db.OrderDetails.ToList();
                List<Order> order = _db.Orders.ToList();
                List<Product> product = _db.Products.ToList();

                // Filter based on UserId. Order by date. Select and group by relevant columns.
                IEnumerable<PurchasesViewModel> purchases =
                    from o in order
                    join od in orderDetail on o.Id equals od.OrderId
                    join p in product on od.ProductId equals p.Id
                    where o.UserId == user.Id
                    orderby o.DateTime descending
                    select new { o.DateTime, od.Id, p.ImageLink, p.Name, p.Description } into y
                    group y by new { y.DateTime, y.ImageLink, y.Name, y.Description } into grp
                    select new PurchasesViewModel
                    {
                        DateTime = grp.Key.DateTime,
                        ImageLink = grp.Key.ImageLink,
                        Name = grp.Key.Name,
                        Quantity = grp.Count(),
                        Description = grp.Key.Description,
                        ActivationCode = grp.Select(x => x.Id).ToList()
                    };

                if (purchases.ToList().Count == 0) // If no purchases, send info to View to display "no past purchases"
                {
                    ViewData["havePastOrders"] = false;

                    return View();
                }

                ViewData["havePastOrders"] = true;

                return View(purchases.ToList());
            }
            else // Else user is not logged in. Redirect to login page.
            {
                TempData["Alert"] = "primary|Please log in to view your purchases.";

                return RedirectToAction("Index", "Login");
            }
        }
    }
}
