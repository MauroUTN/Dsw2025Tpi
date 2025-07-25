using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;


namespace Dsw2025Tpi.Api.Controllers;


     [ApiController]
    [Route("api/products")]



    public class ProductsController : ControllerBase
    {
        private readonly IProductsManagementService _service;
        public ProductsController(IProductsManagementService service)
        {
            _service = service;
        }

        [HttpGet()]

        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetAllProducts();
            if (products == null || !products.Any())
                return NoContent();
            return Ok(products);
        }

      [HttpGet()]

    [Route("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        try
        {
            var product = await _service.GetProductById(id);
            return Ok(product);
        }
        catch (InvalidOperationException ioe)
        {
            return NotFound(ioe.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.requestProductModel request)
    {
        try
        {
            var product = await _service.AddProduct(request);
      //      return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product); 
              return Ok(product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException ioe)
        {
            return BadRequest(ioe.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al guardar el producto");
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.requestProductModel request)
    {
        try
        {
            var product = await _service.UpdateProduct(id, request);
            return Ok(product);
        }
        catch (EntityNotFoundException ioe)
        {
            return NotFound(ioe.Message);
        }
        catch (BadRequestException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Exception ex)
        {
            return Problem($"Error al actualizar el producto: {ex.Message}");
        }
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchProduct(Guid id)
    {
        try
        {
            var product = await _service.PatchProduct(id);
            return Ok(product);
        }
        catch (ApplicationException ioe)
        {
            return NotFound(ioe.Message);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Exception ex)
        {
            return Problem($"Error al actualizar el producto: {ex.Message}");
        }
    }
}

