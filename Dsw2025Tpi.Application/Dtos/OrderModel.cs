using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record OrderModel
    {
        public record RequestOrderModel(string CustomerName, string CustomerEmail, string CustomerPhone, string CustomerAddress, List<OrderItemModel.RequestOrderItemModel> OrderItems);
        public record OrderItemRequestModel(int Quantity, Guid ProductId);
        public record ResponseOrderModel(Guid Id, string CustomerName, string CustomerEmail, string CustomerPhone, string CustomerAddress, DateTime OrderDate, decimal TotalAmount, List<OrderItemModel.ResponseOrderItemModel> OrderItems);
    }
}
