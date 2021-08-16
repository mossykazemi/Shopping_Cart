using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Shopping_Cart.Data;

namespace Shopping_Cart.Jobs
{
    [DisallowConcurrentExecution]
    public class RemoveCartJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var option = new DbContextOptionsBuilder<MyContext>();
            option.UseSqlServer("Server=Mossy;Database=CartDB;Trusted_Connection=True;MultipleActiveResultSets=true");

            using (MyContext _ctx = new MyContext(option.Options))
            {
                var orders = _ctx.Orders.Where(o => o.IsFinally && o.CreateDate < DateTime.Now.AddHours(-24)).ToList();//orders that pass 24 hours
                foreach (var order in orders)
                {
                    var orderDetail = _ctx.OrderDetails.Where(od => od.OrderId == order.OrderId).ToList();
                    foreach (var detail in orderDetail)
                    {
                        _ctx.Remove(detail);
                    }

                    _ctx.Remove(order);
                }

                _ctx.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}
