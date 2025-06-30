using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using StockApp.Application.DTOs;

namespace StockApp.API.Controllers
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
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
        {
            var products = await _productService.GetProductsAsync();
            if (products == null)
            {
                return NotFound("Products not found.");
            }
            return Ok(products);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Post([FromBody] ProductDTO productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Invalid Data.");
            }
            await _productService.Add(productDto);
            return new CreatedAtRouteResult("GetProduct", new { id = productDto.Id }, productDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProductDTO productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest("Invalid id.");
            }
            if (productDto == null)
            {
                return BadRequest("Invalid Data.");
            }
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            await _productService.Update(productDto);
            return Ok(productDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductDTO>> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            await _productService.Remove(id);
            return NoContent();
        }
    }
}