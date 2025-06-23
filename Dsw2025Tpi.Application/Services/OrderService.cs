
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Application.Services;

public class OrderService : IOrderService
{
    private readonly Dsw2025TpiContext _context;

    public OrderService(Dsw2025TpiContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateOrderAsync(OrderCreateDto dto)
    {
        var customer = await _context.Customers.FindAsync(dto.CustomerId);
        if (customer == null) throw new Exception("Cliente no encontrado");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            ShippingAddress = dto.ShippingAddress,
            BillingAddress = dto.BillingAddress,
            Status = OrderStatus.Pending,
            OrderItems = new List<OrderItem>()
        };

        decimal total = 0;
        foreach (var itemDto in dto.Items)
        {
            var product = await _context.Products.FindAsync(itemDto.ProductId);
            if (product == null) throw new Exception($"Producto {itemDto.ProductId} no encontrado");
            if (product.StockQuantity < itemDto.Quantity) throw new Exception($"Stock insuficiente para el producto {product.Sku}");

            product.StockQuantity -= itemDto.Quantity;

            var subtotal = product.CurrentUnitPrice * itemDto.Quantity;
            total += subtotal;

            order.OrderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.CurrentUnitPrice,
                Subtotal = subtotal
            });
        }

        order.TotalAmount = total;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order.Id;
    }
}