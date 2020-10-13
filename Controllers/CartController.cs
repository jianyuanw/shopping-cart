using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class CartController : Controller
    {
        private readonly DbT10Software _db;
        private readonly Verify _v;

        public CartController (DbT10Software _db, Verify v)
        {
            this._db = _db;
            _v = v;
        }
        public IActionResult Index()
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            List<Cart> carts = new List<Cart>();
            //validate session 
            if (_v.VerifySession(sessionId, _db))
            {
                ViewData["Logged"] = true;
                User user = _db.Sessions.FirstOrDefault(s => s.Id == sessionId).User;

                ViewData["Username"] = user.Username;

                //retrieve product number labeled beside icon
                carts = _db.Carts.Where(x => x.UserId == user.Id).ToList();
            }
            else
            {
                string cartCookie = HttpContext.Request.Cookies["guestCart"];
                //tentative cart; now user not log in;
                if (cartCookie != null)
                {
                    var guestCart = JsonSerializer.Deserialize<GuestCart>(HttpContext.Request.Cookies["guestCart"]);
                    carts = guestCart.LoadProducts(_db);
                }
            }

            if (carts.Count == 0)
            {
                // Returns alternate view if there are no items
                ViewData["CartQuantity"] = 0;
                ViewData["Total"] = 0;
                carts = null;
            } else
            {
                ViewData["CartQuantity"] = carts.Sum(cart => cart.Quantity);
                ViewData["ItemsInCart"] = carts;
                ViewData["Total"] = carts.Sum(cart => cart.Quantity * cart.Product.Price);
            }

            return View(carts);

        }

        public IActionResult Checkout(Hasher h)
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];

            if (_v.VerifySession(sessionId, _db))
            {
                User user = _db.Sessions.FirstOrDefault(session => session.Id == sessionId).User;
                List<Cart> carts = _db.Carts.Where(cart => cart.UserId == user.Id).ToList();

                var order = new Order
                {
                    UserId = user.Id,
                    DateTime = DateTime.Now
                };

                foreach (Cart c in carts)
                {
                    for (int i = 0; i < c.Quantity; i++)
                    {
                        var orderDetail = new OrderDetail
                        {
                            Id = h.GenerateActivationKey(_db),
                            Order = order,
                            ProductId = c.ProductId
                        };
                        _db.OrderDetails.Add(orderDetail);
                    }
                    _db.Carts.Remove(c);
                }

                _db.SaveChanges();
                TempData["Alert"] = "primary|Successful checkout!";

            } else
            {
                TempData["ReturnUrl"] = "/Cart/Index";
                TempData["Alert"] = "danger|Login is required to checkout, please login.";
                return Redirect("/Login/Index");
            }
            return Redirect("/Purchase");
        }

        [HttpPost]
        public JsonResult Update(int productId, int quantity)
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            string newPrice;
            string totalPrice;

            if (_v.VerifySession(sessionId, _db))
            {
                User user = _db.Sessions.FirstOrDefault(session => session.Id == sessionId).User;
                Cart cart = _db.Carts.FirstOrDefault(cart => cart.UserId == user.Id && cart.ProductId == productId);

                cart.Quantity = quantity;

                _db.SaveChanges();

                newPrice = (cart.Quantity * cart.Product.Price).ToString();
                totalPrice = (_db.Carts.Where(cart => cart.UserId == user.Id).Sum(cart => cart.Quantity * cart.Product.Price)).ToString();

            } else
            {
                var guestCart = JsonSerializer.Deserialize<GuestCart>(HttpContext.Request.Cookies["guestCart"]);
                double priceSum = 0;

                foreach (var product in guestCart.Products)
                {
                    if (product.ProductId == productId)
                    {
                        product.Quantity = quantity;
                    }
                    priceSum += _db.Products.FirstOrDefault(p => p.Id == product.ProductId).Price * product.Quantity;
                }
                HttpContext.Response.Cookies.Append("guestCart", JsonSerializer.Serialize<GuestCart>(guestCart));

                newPrice = (_db.Products.FirstOrDefault(product => product.Id == productId).Price * quantity).ToString();
                totalPrice = priceSum.ToString();
            }

            return Json(new
            {
                success = true,
                newPrice,
                totalPrice
            });
        }

        [HttpPost]
        public JsonResult Remove(int productId, int row)
        {
            string sessionId = HttpContext.Request.Cookies["sessionId"];
            string totalPrice;

            if (_v.VerifySession(sessionId, _db))
            {
                User user = _db.Sessions.FirstOrDefault(session => session.Id == sessionId).User;
                Cart cart = _db.Carts.FirstOrDefault(cart => cart.UserId == user.Id && cart.ProductId == productId);

                _db.Carts.Remove(cart);

                _db.SaveChanges();
                totalPrice = (_db.Carts.Where(cart => cart.UserId == user.Id).Sum(cart => cart.Quantity * cart.Product.Price)).ToString();
            }
            else
            {
                var guestCart = JsonSerializer.Deserialize<GuestCart>(HttpContext.Request.Cookies["guestCart"]);
                double priceSum = 0;
                Cart productToRemove = null;

                foreach (var product in guestCart.Products)
                {
                    if (product.ProductId == productId)
                    {
                        productToRemove = product;
                        continue;
                    }
                    priceSum += _db.Products.FirstOrDefault(p => p.Id == product.ProductId).Price * product.Quantity;
                }

                if (productToRemove != null)
                {
                    guestCart.Products.Remove(productToRemove);
                }

                HttpContext.Response.Cookies.Append("guestCart", JsonSerializer.Serialize<GuestCart>(guestCart));
                totalPrice = priceSum.ToString();
            }

            return Json(new
            {
                success = true,
                totalPrice,
            });
        }

    }
}
