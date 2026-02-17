using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SexShop.Application.DTOs.Products;
using SexShop.Application.Interfaces;

namespace SexShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Succeeded) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProductDto createProductDto)
        {
            var result = await _productService.CreateProductAsync(createProductDto);
            if (!result.Succeeded) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateProductDto updateProductDto)
        {
            if (id != updateProductDto.Id) return BadRequest("ID Mismatch");
            var result = await _productService.UpdateProductAsync(updateProductDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
