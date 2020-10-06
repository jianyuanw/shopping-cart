using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SA51_CA_Project_Team10.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = TempData["Message"];
            if (HttpContext.Request.Cookies["sessionId"] != null)
            {
                ViewData["Logged"] = true;
            }
            return View();
        }
    }
}
