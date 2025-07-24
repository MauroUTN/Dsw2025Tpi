using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Dtos;



namespace Dsw2025Tpi.Api.Controllers;


    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase  
    {
        private readonly IOrdersManagementService _service;
    public OrdersController(IOrdersManagementService service)
    {
        _service = service;
    }

    [HttpGet()]

    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _service.GetAllOrders();
        if (orders == null || !orders.Any())
            return NotFound("No se encontraron órdenes.");
        return Ok(orders);
    }

    [HttpGet("{id}")] 

    public async Task<IActionResult> GetOrderById(Guid id)
    {   try
        {
            var order = await _service.GetOrderById(id);
            return Ok(order);
        }
        catch (InvalidOperationException ioe)
        {
            return NotFound(ioe.Message);
        }
    }

    [HttpPost]

    public async Task<IActionResult> AddOrder([FromBody] OrderModel.RequestOrderModel request)
    {
        try
        {
            var Order = await _service.AddOrder(request);
            return CreatedAtAction(nameof(GetOrderById), new { id = Order.Id }, Order);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (InvalidOperationException ioe)
        {
            return BadRequest(ioe.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception ex)
        {
            return Problem($"Error al crear la orden: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, string newStatus)
    {
        try
        {
            var order = await _service.PutOrder(id, newStatus);
            if (order == null) return NotFound();
            return Ok(order);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (InvalidOperationException ioe)
        {
            return NotFound(ioe.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception ex)
        {
            return Problem($"Error al actualizar la orden: {ex.Message}");
        }
    }
}

