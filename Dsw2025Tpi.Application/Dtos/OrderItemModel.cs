using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
   public record OrderItemModel
    {
        public record RequestOrderItemModel (int quantity, Guid productId);
        public record ResponseOrderItemModel(Guid Id, int quantity, Guid productId, string productName, decimal unitprice, Guid orderId);
    }
}
