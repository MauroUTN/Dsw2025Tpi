using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    internal class OrderCreateDto
    {
        public Guid CustomerId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string BillingAddress { get; set; } = null!;
        public IList<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
