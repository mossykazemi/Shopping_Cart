using Microsoft.EntityFrameworkCore;
using Shopping_Cart.Models;

namespace Shopping_Cart.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<OrderDetail>()
        //        .HasOne(o => o.Product)
        //        .WithMany(p => p.OrderDetails)
        //        .HasForeignKey(f => f.ProductId);

        //    modelBuilder.Entity<OrderDetail>()
        //        .HasOne(o => o.Order)
        //        .WithMany(od => od.OrderDetails)
        //        .HasForeignKey(f => f.OrderId);

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
