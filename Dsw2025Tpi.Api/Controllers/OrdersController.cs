using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;



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
    [AllowAnonymous]
    public async Task<IActionResult> GetAllOrders()
    {
       var orders = await _service.GetAllOrders();
            if (orders == null || !orders.Any())
                 return NotFound("No se encontraron órdenes.");
            return Ok(orders);
        
    }
    [HttpGet("paged")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPagedOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetOrdersPaged(pageNumber, pageSize);
        if (result == null || !result.Items.Any())
            return NotFound("No se encontraron órdenes.");
        return Ok(result);

    }


    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrderById(Guid id)
    {  
            var order = await _service.GetOrderById(id);
            return Ok(order);

    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]

    public async Task<IActionResult> AddOrder([FromBody] OrderModel.RequestOrderModel request)
    {
            var Order = await _service.AddOrder(request);
            return CreatedAtAction(nameof(GetOrderById), new { id = Order.Id }, Order);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, string newStatus)
    {
            var order = await _service.PutOrder(id, newStatus);
            if (order == null) return NotFound();
            return Ok(order);
    }
}

