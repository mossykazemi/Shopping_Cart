using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shopping_Cart.Data;
using Shopping_Cart.Models.ViewModels;

namespace Shopping_Cart.Components
{
    public class CartComponent : ViewComponent
    {
        private MyContext _context;

        public CartComponent(MyContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<ShowCartViewModel> _list = new List<ShowCartViewModel>();

            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var order = _context.Orders.SingleOrDefault(o => o.UserId == currentUserId && !o.IsFinally);

                if (order != null)
                {
                    var details = _context.OrderDetails.Where(od => od.OrderId == order.OrderId).ToList();
                    foreach (var item in details)
                    {
                        var product = _context.Products.Find(item.ProductId);
                        _list.Add(new ShowCartViewModel()
                        {
                            Count = item.Count,
                            Title = product.Title,
                            ImageName = product.ImageName
                        });
                    }
                }
            }
            return View("/Views/Shared/_ShowCart.cshtml", _list);
        }
    }
}
