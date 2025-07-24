using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext: DbContext
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configuración de entidades
        modelBuilder.Entity<Customer>(i =>
        {
            i.ToTable("Customers");
            i.Property(c => c.Id)
            .ValueGeneratedNever();// No se generará automáticamente, se asignará manualmente
            i.Property(c => c.Email)
            .HasMaxLength(320);
            i.Property(c => c.Name)
            .HasMaxLength(60)
            .IsRequired();
            i.Property(c => c.PhoneNumber);
        });
        modelBuilder.Entity<Product>(i =>
        {
            i.ToTable("Products");
            i.Property(c => c.Id)
            .ValueGeneratedNever();// No se generará automáticamente, se asignará manualmente
            i.Property(c => c.Sku)
            .HasMaxLength(20)
            .IsRequired();
            i.Property(c => c.InternalCode)
            .HasMaxLength(20)
            .IsRequired();
            i.Property(c => c.Name)
            .HasMaxLength(60)
            .IsRequired();
            i.Property(c => c.Description)
            .HasMaxLength(200);
            i.Property(c => c.CurrentUnitPrice)
            .HasPrecision(15, 2);// Precision para precios
            i.Property(c => c.StockQuantity)
            .HasDefaultValue(0);// Valor por defecto para cantidad en stock
        });
        modelBuilder.Entity<Order>(i =>
        {
            i.ToTable("Orders");
            i.Property(c => c.Id)
            .ValueGeneratedNever();
            i.Property(c => c.OrderDate)
            .HasDefaultValueSql("GETDATE()");// Fecha por defecto al momento de la creación
            i.Property(c => c.ShippingAddress)
            .HasMaxLength(200)
            .IsRequired();
            i.Property(c => c.BillingAddress)
            .HasMaxLength(200)
            .IsRequired();
        });
        modelBuilder.Entity<OrderItem>(i => 
        {
            i.ToTable("OrderItems");
            i.Property(c => c.Id)
            .ValueGeneratedNever();
            i.Property(c => c.UnitPrice)
            .HasPrecision(15, 2);
            i.Property(c => c.Quantity)
            .HasDefaultValue(1);// Valor por defecto para cantidad
        });
    }

}
