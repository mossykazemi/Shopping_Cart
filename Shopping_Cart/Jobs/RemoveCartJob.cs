using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Shopping_Cart.Data;

namespace Shopping_Cart.Jobs
{
    [DisallowConcurrentExecution]
    public class RemoveCartJob : IJob
    {
        private MyContext _ctx;

        public RemoveCartJob(MyContext context)
        {
            _ctx = context;
        }
        public Task Execute(IJobExecutionContext context)
        {
            var orders = _ctx.Orders.Where(o => o.IsFinally && o.CreateDate > DateTime.Now.AddHours(-24)).ToList();//orders that pass 24 hours
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
            return Task.CompletedTask;
        }
    }
}
