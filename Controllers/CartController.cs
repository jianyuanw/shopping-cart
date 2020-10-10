﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;

namespace SA51_CA_Project_Team10.Controllers
{
    public class CartController : Controller
    {
        private readonly DbT10Software _db;

        public CartController (DbT10Software _db)
        {
            this._db = _db;
        }
        public IActionResult Index(Verify v)
        {

            //validate session 
            if (v.VerifySession(HttpContext.Request.Cookies["sessionId"], _db))
            {
                ViewData["Logged"] = true;
                int userId = _db.Sessions.Where(x => x.Id == HttpContext.Request.Cookies["sessionId"]).ToList()[0].UserId;
                ViewData["Username"] = _db.Users.Where(x => x.Id == userId).ToList()[0].Username;

                //retrieve product number labeled beside icon
                List<Cart> carts = _db.Carts.Where(x => x.UserId == userId).ToList();
                int total = 0;
                foreach (Cart cart in carts)
                    total += cart.Quantity;
                ViewData["cart_quantity"] = total;

                // pack Cart list and deliver to view; now user logged in;
                ViewData["ItemsInCart"] = carts;
            }
            else
            {  
                //tentative cart; now user not log in;
                if (HttpContext.Request.Cookies["tempCart"] != null)
                {
                    string[] cart = HttpContext.Request.Cookies["tempCart"].Split("*");
                    int sum = 0;
                    foreach (string c in cart)
                        if (c != "" && c != null) ++sum;
                    ViewData["cart_quantity"] = sum;

                    // implement create List <object> and deliver to ViewData["ItemsInCart"]
                    List<Cart> noLoginCart = new List<Cart>();

                    // get productId and merge all same product into List<Cart> noLoginCart
                    // packed static method in this class below: DeriveNoLoginCartListFromCookie
                    noLoginCart = DeriveNoLoginCartListFromCookie(noLoginCart, cart);

                    // implement all information in List<Cart> noLoginCart
                    // packed static method in this class below: FillingAllInformationOfCartObjectBaseOnProductId
                    noLoginCart = FillingAllInformationOfCartObjectBaseOnProductId(noLoginCart);

                    ViewData["ItemsInCart"] = noLoginCart;
                }
                else
                {
                    ViewData["cart_quantity"] = 0;
                }
            }

            // check the cart_quantity, if no item in cart, return no item cart page;
            if((int)ViewData["cart_quantity"] == 0)
                return View("NoItemCart");

            //-----implement cart logic start here(please dont modify others part code)----//
            //implement...
            //implement...
            //-----implement cart logic end here(please dont modify others part code)----//

            //bold navbar 
            ViewData["Is_Cart"] = "font-weight: bold";
            return View("ItemCart");

        }

        public static List<Cart> DeriveNoLoginCartListFromCookie(List<Cart> noLoginCart, string[] cart)
        {
            for (int i = 0; i < cart.Length; i++)
            {
                if (cart[i] != "" && cart[i] != null)
                {
                    int currentCartItem = Convert.ToInt32(cart[i]);
                    for (int j = 0; j < noLoginCart.Count; j++)
                    {
                        if (currentCartItem != noLoginCart[j].ProductId)
                        {
                            noLoginCart.Add(new Cart { ProductId = currentCartItem, Quantity = 1 });
                        }
                        else if (currentCartItem == noLoginCart[j].ProductId)
                        {
                            noLoginCart[j].Quantity += 1;
                        }
                    }
                }
            }

            return noLoginCart;
        }

        public static List<Cart> FillingAllInformationOfCartObjectBaseOnProductId(List<Cart> noLoginCart)
        {
            for(int i = 0; i < noLoginCart.Count; i++)
            {
                int productIDinCode = noLoginCart[i].ProductId;
                noLoginCart[i].Product.Id = productIDinCode;
                // noLoginCart[i].Product = from Products where Id == productIDinCode select *;
            }
            return noLoginCart;
        }

    }
}
