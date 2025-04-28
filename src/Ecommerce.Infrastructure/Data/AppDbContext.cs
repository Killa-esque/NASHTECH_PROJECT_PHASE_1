using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<DiscountCode> DiscountCodes { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Cấu hình Identity tables
        modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");

        // Cấu hình ApplicationUser
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.FullName)
            .HasMaxLength(256);
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.Gender)
            .HasMaxLength(50);
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.DefaultAddress)
            .HasMaxLength(500);
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.AvatarUrl)
            .HasMaxLength(2048);
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.AllergyNotes)
            .HasMaxLength(1000);

        // Cấu hình decimal properties
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);
        modelBuilder.Entity<PaymentTransaction>()
            .Property(pt => pt.Amount)
            .HasPrecision(18, 2);
        modelBuilder.Entity<DiscountCode>()
            .Property(dc => dc.DiscountValue)
            .HasPrecision(18, 2);

        // Product -> Category
        modelBuilder.Entity<Product>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Product>()
            .HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // CartItem -> Product, User
        modelBuilder.Entity<CartItem>()
            .HasKey(ci => ci.Id);
        modelBuilder.Entity<CartItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CartItem>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ci => ci.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Order -> User
        modelBuilder.Entity<Order>()
            .HasKey(o => o.Id);
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>();
        modelBuilder.Entity<Order>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItem -> Order, Product
        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => oi.Id);
        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<OrderItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // PaymentTransaction -> Order
        modelBuilder.Entity<PaymentTransaction>()
            .HasKey(pt => pt.Id);
        modelBuilder.Entity<PaymentTransaction>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(pt => pt.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // DiscountCode
        modelBuilder.Entity<DiscountCode>()
            .HasKey(dc => dc.Id);

        // Rating -> Product, User
        modelBuilder.Entity<Rating>()
            .HasKey(r => r.Id);
        modelBuilder.Entity<Rating>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Rating>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
