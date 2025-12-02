using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;
using Microsoft.AspNetCore.Authorization;


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
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetAllProducts();
            if (products == null || !products.Any())
                return NoContent();
            return Ok(products);
        }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Userss")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
            var product = await _service.GetProductById(id);
            return Ok(product);

    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.requestProductModel request)
    {
            var product = await _service.AddProduct(request);
              return Ok(product);

    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.requestProductModel request)
    {
            var product = await _service.UpdateProduct(id, request);
            return Ok(product);

    }
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchProduct(Guid id)
    {
            var product = await _service.PatchProduct(id);
            return NoContent();

    }
    [HttpGet("paged")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = "", [FromQuery] string status = "todos" )
    {
        var result = await _service.GetProductsPaged(pageNumber, pageSize,name,status);
        return Ok(result);
    }
}

