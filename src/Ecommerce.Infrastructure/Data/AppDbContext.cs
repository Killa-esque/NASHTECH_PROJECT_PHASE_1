using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Data;

public class AppDbContext : DbContext
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

        // Product -> Category
        modelBuilder.Entity<Product>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Product>()
            .HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // CartItem
        modelBuilder.Entity<CartItem>()
            .HasKey(ci => ci.Id);

        modelBuilder.Entity<CartItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order
        modelBuilder.Entity<Order>()
            .HasKey(o => o.Id);

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

        // Rating -> Product
        modelBuilder.Entity<Rating>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Rating>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
