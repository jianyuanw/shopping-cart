﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class GalleryController : Controller
    {
        private readonly DbT10Software _db;
        private readonly Verify _v;

        public GalleryController(DbT10Software db, Verify v) {
            _db = db;
            _v = v;
        }
        public IActionResult Index(string search, int page = 1)
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            User user = null;
            //validate session
            if (_v.VerifySession(sessionId, _db))
            {
                ViewData["Logged"] = true;
                user = _db.Sessions.FirstOrDefault(x => x.Id == sessionId).User;

                ViewData["Username"] = user.Username;

                //retrieve product number labeled beside icon
                List<Cart> carts = _db.Carts.Where(x => x.UserId == user.Id).ToList();
                ViewData["cart_quantity"] = carts.Sum(cart => cart.Quantity);
            }      
            else{  
                //tentative cart
                string cartCookie = HttpContext.Request.Cookies["guestCart"];
                if (cartCookie == null)
                {
                    ViewData["cart_quantity"] = 0;
                }
                else {
                    var guestCart = JsonSerializer.Deserialize<GuestCart>(HttpContext.Request.Cookies["guestCart"]);
                    ViewData["cart_quantity"] = guestCart.Count();
                }
            }

            List<Product> products = new List<Product>();

            //searchbar logic & retrieve products list and pass to view
            if (search.IsNullOrEmpty())
            {
                products = _db.Products.OrderBy(x => x.Name).ToList();
            } else
            {
                products = _db.Products.Where(x => x.Name.Contains(search)).OrderBy(x => x.Name).ToList();
            }

            var galleryView = new GalleryViewModel(user, page, products);

            ViewData["searchbar"] = search;

            return View(galleryView);
        }

        [HttpPost]
        public IActionResult AddCart(int productId) {
            if (_v.VerifySession(HttpContext.Request.Cookies["sessionId"], _db))
            {
                int userid = _db.Sessions.Where(x => x.Id == HttpContext.Request.Cookies["sessionId"]).ToList()[0].UserId;
                List<Cart> cart = _db.Carts.Where(x => x.UserId == userid && x.ProductId == productId).ToList();
                if (cart.Count == 0)
                {
                    _db.Add(new Cart()
                    {
                        Quantity = 1,
                        UserId = userid,
                        ProductId = productId
                    });
                }
                else
                {
                    cart[0].Quantity += 1;
                }
                _db.SaveChanges();

                return Json(new
                {
                    success = true
                });
            } else 
            {
                string cartCookie = HttpContext.Request.Cookies["guestCart"];
                GuestCart guestCart;
                if (cartCookie != null)
                {
                    guestCart = JsonSerializer.Deserialize<GuestCart>(cartCookie);
                }
                else
                {
                    guestCart = new GuestCart();
                }
                guestCart.Add(productId, _db.Products.FirstOrDefault(p => p.Id == productId));
                HttpContext.Response.Cookies.Append("guestCart", JsonSerializer.Serialize<GuestCart>(guestCart));
                return Json(new
                {
                    success = true
                });
            }
        }

        [HttpPost]
        public IActionResult Rate(int rating, int productId)
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];

            if (_v.VerifySession(sessionId, _db))
            {
                User user = _db.Sessions.FirstOrDefault(session => session.Id == sessionId).User;
                _db.Ratings.Add(new Rating
                {
                    UserId = user.Id,
                    ProductId = productId,
                    Score = rating
                });
                _db.SaveChanges();

                return Json(new
                {
                    success = true,
                    newAverage = _db.Ratings.Where(rating => rating.ProductId == productId).Average(rating => rating.Score)
                });
            }
            else
            {
                TempData["Alert"] = "primary|Please log in to rate products.";
                return Json(new
                {
                    success = false,
                    redirect = "/Login/Index"
                });
            }
        }
    }
}
