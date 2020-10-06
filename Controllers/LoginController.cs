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
        public LoginController(DbT10Software db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public string Authenticate(Hasher hasher, string username, string password)
        {
            bool failed = true;
            User user = _db.Users.FirstOrDefault(x => x.Username == username);
            if (user.)
            return $"Username: {username}\nPassword: {hasher.GenerateHashString(password)}";
        }
    }
}
