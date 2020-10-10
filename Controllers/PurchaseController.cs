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

            if (session != null) // If session exists, means user is logged in.
            {
                // Logout button
                ViewData["Logged"] = true;

                // Bold menu item
                ViewData["Is_Purchase"] = "font-weight: bold";

                User user = _db.Users.FirstOrDefault(x => x.Id == session.UserId);

                // Display username at top left
                ViewData["Username"] = user.Username;

                // Retrieve OrderDetails from DB
                List<OrderDetail> orderDetails = _db.OrderDetails.ToList();

                // Filter based on UserId. Order by date. Select and group by relevant columns.
                var purchases = orderDetails.OrderByDescending(od => od.Order.DateTime)
                                            .Where(od => od.Order.UserId == user.Id)
                                            .Select(od => new { od.Product.ImageLink, od.Product.Name, od.Product.Description, od.Order.DateTime, od.Id })
                                            .GroupBy(d => new { d.ImageLink, d.Name, d.Description, d.DateTime }).ToList();

                if (purchases.Count == 0) // If no purchases, send info to View to display "no past purchases"
                {
                    ViewData["havePastOrders"] = false;

                    return View();
                }

                ViewData["havePastOrders"] = true;

                var model = new PurchasesViewModel();

                foreach (var group in purchases)
                {
                    bool product = false;
                    var list = new List<string>();
                    var COD = new ConciseOrderDetail();
                    
                    foreach (var item in group)
                    {
                        if (!product)
                        {                            
                            COD.ImageLink = item.ImageLink;
                            COD.Name = item.Name;
                            COD.Description = item.Description;
                            COD.OrderDate = item.DateTime;
                            COD.Quantity = group.Count();
                            product = true;
                        }
                        list.Add(item.Id);
                    }
                    COD.Ids = list;
                    model._products.Add(COD);
                }

                return View(model);
            }
            else // Else user is not logged in. Redirect to login page.
            {
                TempData["Alert"] = "primary|Please log in to view your purchases.";

                return RedirectToAction("Index", "Login");
            }
        }
    }
}
