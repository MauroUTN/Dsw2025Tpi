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
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService : IOrdersManagementService
    {
        private readonly IRepository _repository;
        // BORRADO: private object _context;  <-- Esto causaba el error CS8618

        public OrdersManagementService(IRepository repository)
        {
            _repository = repository;
        }

        // ... GetOrderById y GetAllOrders se mantienen igual ...
        public async Task<OrderModel.ResponseOrderModel?> GetOrderById(Guid id)
        {
            var order = await _repository.GetById<Order>(id, nameof(Order.OrderItems), "OrderItems.Product", "Customer");

            if (order == null)
                throw new EntityNotFoundException($"Orden {id} no fue encontrada");

            return new OrderModel.ResponseOrderModel(
                order.Id, order.OrderDate, order.ShippingAddress, order.BillingAddress,
                order.Notes, order.CustomerId, order.Status,
                order.Customer?.Name ?? "Cliente Desconocido");
        }

        public async Task<IEnumerable<OrderModel.ResponseOrderModel>?> GetAllOrders()
        {
            var activeOrders = await _repository.GetFiltered<Order>(o => !o.Status.Equals(5), "Customer");
            // Usamos '?? Enumerable.Empty<Order>()' para asegurar que no sea nulo
            var safeOrders = activeOrders ?? Enumerable.Empty<Order>();

            return safeOrders.Select(o => new OrderModel.ResponseOrderModel(
                o.Id, o.OrderDate, o.ShippingAddress, o.BillingAddress,
                o.Notes, o.CustomerId, o.Status,
                o.Customer?.Name ?? "Cliente Desconocido"));
        }

        public async Task<OrderModel.ResponseOrderModel> AddOrder(OrderModel.RequestOrderModel request)
        {
            // 1. Validamos (Esto asegura que ShippingAddress NO es null)
            OrderValidator.Validate(request);

            if (request.OrderItems == null || !request.OrderItems.Any())
                throw new ArgumentException("La orden debe tener al menos un item.");

            // 2. Creamos la orden
            // Usamos '!' en request.ShippingAddress! para decirle al compilador: 
            // "Confía en mí, el validador de arriba ya chequeó que esto no es null".
            var order = new Order(
                request.OrderDate,
                request.ShippingAddress!,
                request.BillingAddress!,
                request.CustomerId,
                request.Notes // Notes ya acepta null en el constructor arreglado
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

            var customer = await _repository.GetById<Customer>(request.CustomerId);
            var customerName = customer?.Name ?? "Cliente Nuevo";

            return new OrderModel.ResponseOrderModel(
                order.Id,
                order.OrderDate,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CustomerId,
                order.Status,
                customerName
            );
        }

        // ... El resto de métodos (PutOrder, GetOrdersPaged) se mantienen igual ...
        // Solo asegúrate en GetOrdersPaged de usar el null coalescing si 'query' pudiera ser null
        public async Task<OrderModel.ResponseOrderModel> PutOrder(Guid id, string newStatus)
        {
            var exist = await _repository.First<Order>(o => o.Id == id, "Customer");
            if (exist == null)
                throw new EntityNotFoundException($"No se encontró la orden con ID: {id}");

            var status = Enum.Parse<OrderStatus>(newStatus.ToUpper());
            if (!Enum.IsDefined(typeof(OrderStatus), status))
                throw new ArgumentException($"El estado de la orden '{newStatus}' no es válido.");

            exist.Status = status;
            await _repository.Update(exist);

            return new OrderModel.ResponseOrderModel(
                exist.Id,
                exist.OrderDate,
                exist.ShippingAddress,
                exist.BillingAddress,
                exist.Notes,
                exist.CustomerId,
                exist.Status,
                exist.Customer?.Name ?? "Cliente Desconocido"
            );
        }

        public async Task<PagedResult<OrderModel.ResponseOrderModel>> GetOrdersPaged(
     int pageNumber,
     int pageSize,
     Guid? orderId,
     string status,
     string searchTerm
 )
        {
            var query = (await _repository.GetAll<Order>("Customer")).AsQueryable();

            // ... (Tus filtros de orderId y status se quedan igual) ...
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
                // --- CAMBIO AQUÍ ---
                // Cambiamos "p.Customer?.Name" por la versión compatible con Expression Trees:
                p.Customer != null ? p.Customer.Name : "Cliente Desconocido"
            ))
            .ToList();

            return new PagedResult<OrderModel.ResponseOrderModel>(items, totalCount, pageNumber, pageSize);
        }
    }
}