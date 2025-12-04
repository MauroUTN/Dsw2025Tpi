using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Validation;

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
            // Incluimos OrderItems para el total y Customer para el nombre
            var order = await _repository.GetById<Order>(id, "OrderItems", "Customer");

            if (order == null)
                throw new EntityNotFoundException($"Orden {id} no fue encontrada");

            return MapToResponse(order);
        }

        public async Task<IEnumerable<OrderModel.ResponseOrderModel>?> GetAllOrders()
        {
            var activeOrders = await _repository.GetFiltered<Order>(o => !o.Status.Equals(5), "Customer", "OrderItems");
            var safeOrders = activeOrders ?? Enumerable.Empty<Order>();

            return safeOrders.Select(MapToResponse);
        }

        public async Task<OrderModel.ResponseOrderModel> AddOrder(OrderModel.RequestOrderModel request)
        {
            OrderValidator.Validate(request);

            if (request.OrderItems == null || !request.OrderItems.Any())
                throw new ArgumentException("La orden debe tener al menos un item.");

            var order = new Order(
                request.OrderDate,
                request.ShippingAddress!,
                request.BillingAddress!,
                request.CustomerId,
                request.Notes
            );

            await _repository.Add(order);

            var orderItems = new List<OrderItem>();

            foreach (var item in request.OrderItems)
            {
                var product = await _repository.GetById<Product>(item.ProductId)
                    ?? throw new InvalidOperationException($"Producto no encontrado: {item.ProductId}");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Stock insuficiente para el producto: {product.Name}");

                if (!product.IsActive)
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
            }

            order.OrderItems = orderItems;
            await _repository.Update(order);

            // Recargamos la orden completa con Cliente e Items para devolverla bien
            var fullOrder = await _repository.GetById<Order>(order.Id, "Customer", "OrderItems");
            return MapToResponse(fullOrder!);
        }

        public async Task<OrderModel.ResponseOrderModel> PutOrder(Guid id, string newStatus)
        {
            var exist = await _repository.First<Order>(o => o.Id == id, "Customer", "OrderItems");
            if (exist == null)
                throw new EntityNotFoundException($"No se encontró la orden con ID: {id}");

            var status = Enum.Parse<OrderStatus>(newStatus.ToUpper());
            if (!Enum.IsDefined(typeof(OrderStatus), status))
                throw new ArgumentException($"El estado de la orden '{newStatus}' no es válido.");

            exist.Status = status;
            await _repository.Update(exist);

            return MapToResponse(exist);
        }

        public async Task<PagedResult<OrderModel.ResponseOrderModel>> GetOrdersPaged(
            int pageNumber,
            int pageSize,
            Guid? orderId,
            string status,
            string searchTerm // <--- Este es el dato que ahora llegará desde el front
        )
        {
            var allOrders = await _repository.GetAll<Order>("Customer", "OrderItems");
            var query = (allOrders ?? Enumerable.Empty<Order>()).AsQueryable();

            if (orderId.HasValue)
            {
                query = query.Where(o => o.Id == orderId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status) && !status.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(o => o.Status == parsedStatus);
                }
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (Guid.TryParse(searchTerm, out var parsedId))
                {
                    query = query.Where(o => o.Id == parsedId || (o.Customer != null && o.Customer.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    query = query.Where(o => o.Customer != null && o.Customer.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }
            }

            var totalCount = query.Count();

            var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new OrderModel.ResponseOrderModel(
                p.Id,
                p.OrderDate,
                p.ShippingAddress,
                p.BillingAddress,
                p.Notes,
                p.CustomerId,
                p.Status,
                p.OrderItems != null ? p.OrderItems.Sum(i => i.UnitPrice * i.Quantity) : 0,
                p.Customer != null ? p.Customer.Name : "Cliente Desconocido"
            ))
            .ToList();

            return new PagedResult<OrderModel.ResponseOrderModel>(items, totalCount, pageNumber, pageSize);
        }

        // Método auxiliar para mapear y evitar repetir código
        private OrderModel.ResponseOrderModel MapToResponse(Order order)
        {
            return new OrderModel.ResponseOrderModel(
                order.Id,
                order.OrderDate,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CustomerId,
                order.Status,
                // Calculamos el total sumando (Precio * Cantidad) de cada item
                order.OrderItems?.Sum(i => i.UnitPrice * i.Quantity) ?? 0,
                // Obtenemos el nombre del cliente de forma segura
                order.Customer != null ? order.Customer.Name : "Cliente Desconocido"
            );
        }
    }
}
