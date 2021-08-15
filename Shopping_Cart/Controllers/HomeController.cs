using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shopping_Cart.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Shopping_Cart.Data;
using ZarinpalSandbox;

namespace Shopping_Cart.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MyContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_context.Products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult OnlinePayment(int id)//id is OrderId
        {
            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" &&
                HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"].ToString();
                var order = _context.Orders.Find(id);
                var payment = new Payment(order.Sum);
                var res = payment.Verification(authority).Result;
                if (res.Status == 100)
                {
                    order.IsFinally = true;
                    _context.Orders.Update(order);
                    _context.SaveChanges();
                    ViewBag.Code = res.RefId;
                    return View();
                }
            }

            return NotFound();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
