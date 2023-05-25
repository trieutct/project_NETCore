using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sport_Shop.Areas.Admin.Models;
using Sport_Shop.Areas.Customer.Models;

using System;
using System.Linq;
using System.Xml;

namespace Sport_Shop.Models
{
    public class ShopSportDbConText : DbContext
    {
        public ShopSportDbConText(DbContextOptions options) : base(options)
        {
        }
        public DbSet<EmployeeAccount> EmployeeAccount { get; set; }
        public DbSet<Emoloyee> Employee { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductSize> productSizes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        //public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(p => p.CustomerId);
            });
            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("Order");
                e.HasKey(p => new { p.OrderId});
                e.HasOne(e=> e.customer).WithMany(p=>p.orders).HasForeignKey(e=>e.CustomerId);
            });
            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.ToTable("OrderDetail");
                e.HasKey(p => new {p.OrderDetailId});
            });
            
            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.HasOne(x=>x.Order).WithMany(p=>p.orderDetails).HasForeignKey(x=>x.OrderId);
            });
            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.HasOne(x => x.Product).WithMany(p => p.orderDetails).HasForeignKey(x => x.ProductId);
            });
            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.HasOne(x => x.size).WithMany(p => p.orderDetails).HasForeignKey(x => x.SizeId);
            });



            modelBuilder.Entity<ProductSize>(e =>
            {
                e.ToTable("ProductSize");
                e.HasKey(p => new {p.ProductId,p.SizeId,p.ProductSizeId});
            });
            /*modelBuilder.Entity<Cart>(e =>
            {
                e.ToTable("Cart");
                e.HasKey(p => new {p.ProductId,p.CustomerId});
            });*/
            modelBuilder.Entity<ProductSize>(e =>
            {
                e.HasOne(p => p.product).WithMany(x => x.productSizes).HasForeignKey(p => p.ProductId);
            });
            modelBuilder.Entity<ProductSize>(e =>
            {
                e.HasOne(p => p.size).WithMany(x => x.productSizes).HasForeignKey(p => p.SizeId);
            });

            modelBuilder.Entity<Size>(e =>
            {
                e.ToTable("Size");
                e.Property(p => p.TenSize).HasColumnType("NVARCHAR(20)");
                e.HasIndex(p => p.TenSize).IsUnique();
            });
        }


    }
}
