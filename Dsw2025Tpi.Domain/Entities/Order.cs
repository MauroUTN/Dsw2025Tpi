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
            OrderDate = orderDate;
            ShippingAddress = shippingAdress;
            BillingAddress = billingAdress;
            CustomerId = customerId;
            Notes = notes;
            Status = OrderStatus.PENDING;
            OrderItems = [];
        }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public decimal TotalAmount => OrderItems.Sum(item => item.Subtotal);
        public string Notes { get; set; }
        //relaciones
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
  
}
