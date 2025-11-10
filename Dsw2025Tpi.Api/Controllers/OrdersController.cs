using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;



namespace Dsw2025Tpi.Api.Controllers;


    [ApiController]
    [Route("api/orders")]
    [Authorize]
public class OrdersController : ControllerBase  

    {
        private readonly IOrdersManagementService _service;
    public OrdersController(IOrdersManagementService service)
    {
        _service = service;
    }

    [HttpGet()]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllOrders()
    {
        try
        {

            var orders = await _service.GetAllOrders();
            if (orders == null || !orders.Any())
                 return NotFound("No se encontraron órdenes.");
            return Ok(orders);
        }
        catch(Application.Exceptions.ApplicationException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpGet("paged")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPagedOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _service.GetOrdersPaged(pageNumber, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound("No se encontraron órdenes para esta página.");

            return Ok(result);
        }
        catch (Application.Exceptions.ApplicationException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Exception e)
        {
            return Problem($"Error al obtener las órdenes paginadas: {e.Message}");
        }
    }


    [HttpGet("{id}")]
    [AllowAnonymous]
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
    [Authorize(Roles = "User,Admin")]

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
    [Authorize(Roles = "Admin")]
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

