using curdinStoredprocedure.DataAccessLayer;
using curdinStoredprocedure.Filter;
using curdinStoredprocedure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace curdinStoredprocedure.Controllers
{
    [ApiController]
    [Route("api/productitems")]
    public class ProductItemController : Controller
    {
        private readonly ProductItemsData _productItemData;

        public ProductItemController(ProductItemsData productItemsData)
        {
            _productItemData = productItemsData;
        }

        
        [HttpGet]
        public IActionResult GetProductItems()
        {
            var productItems = _productItemData.GetAllProductItems();
            return Ok(productItems);
        }

        // Get a specific product item by ID
        [HttpGet("{id}")]
        public IActionResult GetProductItemById(int id)
        {
            var productItem = _productItemData.GetProductItemById(id);
            if (productItem != null)
            {
                return Ok(productItem);
            }
            return NotFound(new { message = "Product item not found." });
        }

        // Create a new product item
        [HttpPost]
        public IActionResult CreateProductItem([FromBody] ProductItems model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int newProductId = _productItemData.InsertProductItem(model);
                    return CreatedAtAction(nameof(GetProductItemById), new { id = newProductId }, model);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = $"An error occurred: {ex.Message}" });
                }
            }
            return BadRequest(ModelState);
        }

        // Update an existing product item
        [HttpPut("{id}")]
        public IActionResult UpdateProductItem(int id, [FromBody] ProductItems model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProductItem = _productItemData.GetProductItemById(id);
                    if (existingProductItem == null)
                    {
                        return NotFound(new { message = "Product item not found." });
                    }

                    _productItemData.UpdateProductItem(model);
                    var producrst = model.ProductName;
                    return Ok(new { message = "Product updated successfully." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = $"An error occurred: {ex.Message}" });
                }
            }
            return BadRequest(ModelState);
        }

        // Delete a product item
        [HttpDelete("{id}")]
        public IActionResult DeleteProductItem(int id)
        {
            try
            {
                var existingProductItem = _productItemData.GetProductItemById(id);
                if (existingProductItem == null)
                {
                    return NotFound(new { message = "Product item not found." });
                }

                _productItemData.DeleteProductItem(id);
                return Ok(new { message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }

        // Get product items by category
        //[UserAuthorize]
        [HttpGet("category/{categoryId}")]
        public IActionResult GetProductItemsByCategory(int categoryId)
        {
            var productItems = _productItemData.GetProductItemsByCategory(categoryId);
            if (productItems == null || !productItems.Any())
            {
                return NotFound(new { message = "No products found for this category." });
            }
            return Ok(productItems);
        }
    }
}
