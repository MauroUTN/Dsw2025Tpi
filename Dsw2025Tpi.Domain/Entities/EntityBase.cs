namespace Dsw2025Tpi.Domain.Entities;

// Base común
public abstract class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

// Producto
public class Product : EntityBase
{
    public string Sku { get; set; } = null!;
    public string InternalCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal CurrentUnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

// Orden
public class Order : EntityBase
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!; // propiedad de navegación

    public string ShippingAddress { get; set; } = null!;
    public string BillingAddress { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}


// Ítem de la orden
public class OrderItem : EntityBase
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}


// Estado de la orden
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

