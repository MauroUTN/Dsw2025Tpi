using System;
using System.Collections.Generic;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Dtos
{
    public record OrderModel
    {
        public record RequestOrderModel(
            DateTime OrderDate,
            string? ShippingAddress,
            string? BillingAddress,
            string? Notes,
            Guid CustomerId,
            List<OrderItemModel.RequestOrderItemModel> OrderItems,
            OrderStatus Status
        );

        public record OrderItemRequest(Guid ProductId, int Quantity);

        // CAMBIO: Agregamos 'TotalAmount' y 'CustomerName'
        public record ResponseOrderModel(
            Guid Id,
            DateTime OrderDate,
            string? ShippingAddress,
            string? BillingAddress,
            string? Notes,
            Guid CustomerId,
            OrderStatus Status,
            decimal TotalAmount,   // <--- Nuevo
            string CustomerName    // <--- Nuevo
        );

        public record SearchOrder(Guid? CustomerId, string? Status, int PageNumber = 1, int PageSize = 10);
    }
}