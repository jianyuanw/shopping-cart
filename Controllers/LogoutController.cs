using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class LogoutController : Controller
    {
        private readonly DbT10Software _db;
        public LogoutController(DbT10Software db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];

            _db.Sessions.Remove(new Session()
            {
                Id = sessionId
            });
            _db.SaveChanges();

            HttpContext.Response.Cookies.Delete("sessionId");

            TempData["Alert"] = "primary|Successfully logged out!";
            return Redirect("/Gallery/Index");
        }
    }
}
