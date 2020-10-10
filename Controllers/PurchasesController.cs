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
    public class PurchasesController : Controller
    {
        private readonly DbT10Software _db;
        private readonly Verify verify;

        public PurchasesController(DbT10Software db, Verify verify)
        {
            _db = db;
            this.verify = verify;
        }
        public IActionResult Index()
        {

            //If currUser not null, means logged in
            if (HttpContext.Request.Cookies["sessionId"] != null && verify.VerifySession(HttpContext.Request.Cookies["sessionId"], _db))
            {
                //Settle all _layout stuff first
                //Display name
                int userId = _db.Sessions.Where(x => x.Id == HttpContext.Request.Cookies["sessionId"]).ToList()[0].UserId;
                ViewData["Username"] = _db.Users.Where(x => x.Id == userId).ToList()[0].Username;
                //Login/Logout
                ViewData["Logged"] = true;
                //Bold puchases when clicked
                ViewData["Is_Purchases"] = "font-weight: bold";

                //Get all data as lists - need orderDetail, product, order
                List<OrderDetail> orderDetail = _db.OrderDetails.ToList();
                List<Product> product = _db.Products.ToList();
                List<Order> order = _db.Orders.ToList();

                //Implement sql logic here - need to create a viewmodel class
                IEnumerable<PurchasesViewModel> table =
                    from o in order
                    join od in orderDetail on o.Id equals od.OrderId
                    join p in product on od.ProductId equals p.Id
                    where o.UserId == userId
                    select new { o.DateTime, od.Id, p.ImageLink, p.Name, p.Description } into y
                    group y by new { y.DateTime, y.ImageLink, y.Name, y.Description } into grp
                    select new PurchasesViewModel {
                        DateTime = grp.Key.DateTime,
                        ImageLink = grp.Key.ImageLink,
                        Name = grp.Key.Name,
                        Quantity = grp.Count(),
                        Description = grp.Key.Description,
                        ActivationCode = grp.Select(x => x.Id).ToList()
                    };


                return View(table.ToList());
            }
            else
                return RedirectToAction("Index", "Login");

            
        }
    }
}
