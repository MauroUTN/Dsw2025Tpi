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


    [HttpGet("{id}")] //comentario el que va adentro (dini desconfia)

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
        catch (Exception ex)
        {
            return BadRequest($"Error al crear la orden: {ex.Message}");
        }
    }


}

