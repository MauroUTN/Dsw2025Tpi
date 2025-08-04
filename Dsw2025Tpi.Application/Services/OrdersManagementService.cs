using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Validation;
using Dsw2025Tpi.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService : IOrdersManagementService
    {
        private readonly IRepository _repository;
        public OrdersManagementService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderModel.ResponseOrderModel?> GetOrderById(Guid id)
        {
            var order = await _repository.GetById<Order>(id, nameof(Order.OrderItems), "OrderItems.Product");
            if (order == null)
                throw new EntityNotFoundException($"Orden {id} no fue encontrada");
            return order != null ?
                new OrderModel.ResponseOrderModel(order.Id, order.OrderDate, order.ShippingAddress, order.BillingAddress, order.Notes, order.CustomerId, order.Status) :
                null;
        }
        public async Task<IEnumerable<OrderModel.ResponseOrderModel>?> GetAllOrders()
        {

            var activeOrders = await _repository
                .GetFiltered<Order>(o => !o.Status.Equals(5)) ?? throw new Application.Exceptions.ApplicationException("No hay ordenes no canceladas");
            return (activeOrders
                .Select(o => new OrderModel.ResponseOrderModel(o.Id, o.OrderDate, o.ShippingAddress, o.BillingAddress, o.Notes, o.CustomerId, o.Status)));
        }

        public async Task<OrderModel.ResponseOrderModel> AddOrder(OrderModel.RequestOrderModel request)
        { 
            OrderValidator.Validate(request);

            if (request.OrderItems == null || !request.OrderItems.Any())
                throw new ArgumentException("La orden debe tener al menos un item.");

            var order = new Order(
                request.OrderDate,
                request.ShippingAddress,
                request.BillingAddress,
                request.CustomerId,
                request.Notes
            );

            await _repository.Add(order);

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in request.OrderItems)
            {
                var product = await _repository.GetById<Product>(item.ProductId)
                    ?? throw new InvalidOperationException($"Producto no encontrado: {item.ProductId}");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Stock insuficiente para el producto: {product.Name}");
                
                if(!product.IsActive)
                    throw new InvalidOperationException($"El producto {product.Name} no esta activo");

                product.StockQuantity -= item.Quantity;
                await _repository.Update(product);

                var orderItem = new OrderItem(
                    product.CurrentUnitPrice,
                    item.Quantity,
                    order.Id,
                    product.Id
                );
                orderItems.Add(orderItem);
                totalAmount += product.CurrentUnitPrice * item.Quantity;
            }

            order.OrderItems = orderItems;
            await _repository.Update(order);

            var responseItems = orderItems.Select(oi => new OrderItemModel.ResponseOrderItemModel(
                oi.Id,
                oi.Quantity,
                oi.UnitPrice,
                oi.OrderId,
                oi.ProductId
            )).ToList();

            return new OrderModel.ResponseOrderModel(
                order.Id,
                order.OrderDate,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CustomerId,
                order.Status
            );
        }

        public async Task<OrderModel.ResponseOrderModel> PutOrder(Guid id , string newStatus)
        {
            var exist = await _repository.First<Order>(o => o.Id == id);
            if (exist == null)
                throw new EntityNotFoundException($"No se encontró la orden con ID: {id}");
            var status = Enum.Parse<OrderStatus>(newStatus.ToUpper());
            if (!Enum.IsDefined(typeof(OrderStatus), status))
                throw new ArgumentException($"El estado de la orden '{newStatus}' no es válido.");
            exist.Status = status;

            await _repository.Update(exist);

            return new OrderModel.ResponseOrderModel
           (
                exist.Id,
                exist.OrderDate,
                exist.ShippingAddress,
                exist.BillingAddress,
                exist.Notes,
                exist.CustomerId,
                exist.Status
            );

        }
    }
}
