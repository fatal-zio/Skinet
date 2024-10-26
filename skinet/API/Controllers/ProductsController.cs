using System.Collections.Generic;
using Core.Entities;
using Core.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductRepository repository) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            return Ok(await repository.GetProductsAsync(brand, type, sort));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repository.GetProductByIdAsync(id);

            return (product == null) ? NotFound() : product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProducts(Product product)
        {
            repository.AddProduct(product);

            return (await repository.SaveChangesAsync()) ? 
                CreatedAtAction("GetProduct", new { id = product.Id }, product) : 
                BadRequest("Problem creating product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || !ProductExits(id))
                return BadRequest("Cannot update this product");

            repository.UpdateProduct(product);

            return (await repository.SaveChangesAsync()) ? 
                NoContent() : 
                BadRequest("Problem updating the product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await repository.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            repository.DeleteProduct(product);

            return (await repository.SaveChangesAsync()) ? 
                NoContent() : 
                BadRequest("Problem deleting the product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repository.GetBrandsAsync());
        } 

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repository.GetTypesAsync());
        } 

        private bool ProductExits(int id)
        {
            return repository.ProductExists(id);
        }
    }
}