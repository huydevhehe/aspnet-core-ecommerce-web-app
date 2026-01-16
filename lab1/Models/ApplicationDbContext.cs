using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using lab1.Models;

namespace lab1.Models
{
    public class ApplicationDbContext : IdentityDbContext // Kế thừa từ IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShippingModel> Shippings { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Xử lý lỗi PK_AspNetUserTokens bị ràng buộc trên cột Name
            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            // Định dạng decimal(18,2) chỉ cho cột Price
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ShippingModel>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            // Đảm bảo CategoryId không bị ép kiểu decimal
            modelBuilder.Entity<Product>()
                .Property(p => p.CategoryId)
                .HasColumnType("int");
        }

    }
}
