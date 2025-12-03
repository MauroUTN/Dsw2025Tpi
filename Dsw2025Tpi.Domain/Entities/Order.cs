using System;
using System.Collections.Generic;
using System.Linq;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }
        public Order(DateTime orderDate, string shippingAdress, string billingAdress, Guid customerId, string? notes)
        {
            CustomerId = customerId;
            OrderDate = orderDate;
            ShippingAddress = shippingAdress;
            BillingAddress = billingAdress;
            Notes = notes;
            Status = OrderStatus.PENDING;
            OrderItems = new List<OrderItem>();
        }

        public DateTime OrderDate { get; set; }

        public string ShippingAddress { get; set; } = null!;
        public string BillingAddress { get; set; } = null!;

        public string? Notes { get; set; } 

        public decimal TotalAmount => OrderItems != null ? OrderItems.Sum(p => p.Subtotal) : 0;

        public OrderStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}