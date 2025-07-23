using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public Order(DateTime orderDate , string shippingAdress , string billingAdress ,Guid customerId , string notes )
        {
            CustomerId = customerId;
            OrderDate = orderDate;
            ShippingAddress = shippingAdress;
            BillingAddress = billingAdress;
            Notes = notes;
            Status = OrderStatus.PENDING;
            OrderItems = [];
        }
        public DateTime OrderDate { get; set; }
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount => OrderItems.Sum(p => p.Subtotal);


        public OrderStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
  
}
