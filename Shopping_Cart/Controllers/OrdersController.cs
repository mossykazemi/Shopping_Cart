﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Shopping_Cart.Data;
using Shopping_Cart.Models;

namespace Shopping_Cart.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private MyContext _context;

        public OrdersController(MyContext context)
        {
            _context = context;
        }


        public IActionResult AddToCart(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);//User id
            Order order = _context.Orders.SingleOrDefault(o => o.UserId == currentUserId && !o.IsFinally);//check to see if user have an Open Order

            if (order == null)
            {
                order = new Order
                {
                    UserId = currentUserId,
                    CreateDate = DateTime.Now,
                    IsFinally = false,
                    Sum = 0
                };
                _context.Orders.Add(order);
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    Count = 1,
                    Price = _context.Products.Find(id).Price,
                    ProductId = id
                });
                _context.SaveChanges();
            }
            else
            {
                var details =
                    _context.OrderDetails.SingleOrDefault(od => od.OrderId == order.OrderId && od.ProductId == id);
                if (details == null)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.OrderId,
                        Count = 1,
                        Price = _context.Products.Find(id).Price,
                        ProductId = id
                    });
                }
                else
                {
                    details.Count += 1;
                    _context.Update(details);
                }

                _context.SaveChanges();
            }
            return Redirect("/");
        }
    }
}
