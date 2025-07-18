using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Interfaces
{
    internal interface IOrdersManagementService
    {
        Task<OrderModel.ResponseOrderModel?> GetOrderById(Guid id);
        Task<IEnumerable<OrderModel.ResponseOrderModel>?> GetAllOrders();
        Task<OrderModel.ResponseOrderModel> AddOrder(OrderModel.RequestOrderModel request);
        Task<OrderModel.ResponseOrderModel> PutOrder(OrderModel.RequestOrderModel request);
    }
}
