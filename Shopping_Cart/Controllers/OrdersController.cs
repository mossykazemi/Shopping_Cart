﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Shopping_Cart.Data;
using Shopping_Cart.Models;
using Shopping_Cart.Models.ViewModels;
using ZarinpalSandbox;

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


        public IActionResult AddToCart(int id) //Product id
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);//User id
            Order order = _context.Orders.SingleOrDefault(o => o.UserId == currentUserId && !o.IsFinally);//check to see if user have an Open Order

            if (order == null)
            {
                order = new Order()
                {
                    UserId = currentUserId,
                    CreateDate = DateTime.Now,
                    IsFinally = false,
                    Sum = 0
                };
                _context.Add(order);
                _context.SaveChanges();

                _context.OrderDetails.Add(new OrderDetail()
                {
                    OrderId = order.OrderId,
                    Count = 1,
                    Price = _context.Products.Find(id).Price,
                    ProductId = id,
                });
                //_context.OrderDetails.Add(od);
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
            UpdateSumOrder(order.OrderId);
            return Redirect("/");
        }


        public IActionResult ShowOrder()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Order order = _context.Orders.SingleOrDefault(o => o.UserId == currentUserId && !o.IsFinally);

            List<ShowOrderViewModel> _list = new List<ShowOrderViewModel>();
            if (order != null)
            {
                var details = _context.OrderDetails.Where(od => od.OrderId == order.OrderId).ToList();
                foreach (var item in details)
                {
                    var product = _context.Products.Find(item.ProductId);
                    _list.Add(new ShowOrderViewModel
                    {
                        Count = item.Count,
                        ImageName = product.ImageName,
                        OrderDetailId = item.OrderDetailId,
                        Price = item.Price,
                        Sum = item.Count * item.Price,
                        Title = product.Title
                    });
                }
            }

            return View(_list);
        }

        public IActionResult Delete(int id)
        {
            var orderDetail = _context.OrderDetails.Find(id);
            _context.Remove(orderDetail);
            _context.SaveChanges();
            return RedirectToAction("ShowOrder");
        }

        public IActionResult Command(int id, string command)
        {
            var orderDetail = _context.OrderDetails.Find(id);

            switch (command)
            {
                case "up":
                    {
                        orderDetail.Count += 1;
                        _context.Update(orderDetail);
                        break;
                    }
                case "down":
                    {
                        orderDetail.Count -= 1;
                        if (orderDetail.Count == 0)
                        {
                            _context.OrderDetails.Remove(orderDetail);
                        }
                        else
                        {
                            _context.Update(orderDetail);
                        }
                        break;
                    }
            }

            _context.SaveChanges();
            return RedirectToAction("ShowOrder");
        }

        public void UpdateSumOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            order.Sum = _context.OrderDetails.Where(od => od.OrderId == order.OrderId).Select(d => d.Count * d.Price).Sum();
            _context.Update(order);
            _context.SaveChanges();
        }

        public IActionResult Payment()
        {
            var order = _context.Orders.SingleOrDefault(o => !o.IsFinally);
            if (order == null)
                return NotFound();

            var payment = new Payment(order.Sum);
            var res = payment.PaymentRequest($"پرداخت فاکتور شماره {order.OrderId}",
                "https://localhost:44363/Home/OnlinePayment/" + order.OrderId,
                "m.kazemi.512@gmail.com","09217701362");
            if (res.Result.Status==100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }
            else
            {
                return BadRequest();//or we can set optional message here 
            }
            return null;
        }
    }
}
