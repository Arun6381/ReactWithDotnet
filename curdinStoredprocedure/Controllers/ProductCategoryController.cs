using curdinStoredprocedure.DataAccessLayer;
using curdinStoredprocedure.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProductCategoryApp.Controllers
{
    [ApiController]
    [Route("api/productcategories")]
    public class ProductCategoryController : Controller
    {
        private readonly ProductCategoryData _productCategoryData;

        public ProductCategoryController(ProductCategoryData productCategoryData)
        {
            _productCategoryData = productCategoryData;
        }

        [HttpGet]
        public IActionResult GetProductCategories()
        {
            var categories = _productCategoryData.GetAllProductCategories();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductCategoryById(int id)
        {
            var category = _productCategoryData.GetProductCategoryById(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }
            return Ok(category);
        }

        // Create a new product category (API)
        [HttpPost]
        public IActionResult CreateProductCategory([FromBody] Productcategory model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int newCategoryId = _productCategoryData.InsertProductCategory(model.CategoryName, model.Description);
                    return CreatedAtAction(nameof(GetProductCategoryById), new { id = newCategoryId }, model);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = $"An error occurred: {ex.Message}" });
                }
            }
            return BadRequest(ModelState);
        }

        // Update an existing product category (API)
        [HttpPut("{id}")]
        public IActionResult UpdateProductCategory(int id, [FromBody] Productcategory model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = _productCategoryData.GetProductCategoryById(id);
                    if (existingCategory == null)
                    {
                        return NotFound(new { message = "Category not found." });
                    }

                    _productCategoryData.UpdateProductCategory(id, model.CategoryName, model.Description);
                    return Ok(new { message = "Category updated successfully!" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = $"An error occurred: {ex.Message}" });
                }
            }
            return BadRequest(ModelState);
        }

        // Delete a product category (API)
        [HttpDelete("{id}")]
        public IActionResult DeleteProductCategory(int id)
        {
            try
            {
                var existingCategory = _productCategoryData.GetProductCategoryById(id);
                if (existingCategory == null)
                {
                    return NotFound(new { message = "Category not found." });
                }

                _productCategoryData.DeleteProductCategory(id);
                return Ok(new { message = "Category deleted successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}

//        // Default MVC Index
//        [HttpGet("~/productcategories")]
//        public IActionResult Index()
//        {
//            var categories = _productCategoryData.GetAllProductCategories();
//            return View(categories);
//        }

//        // Default MVC Create (GET)
//        [HttpGet("~/productcategories/create")]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // Default MVC Create (POST)
//        [HttpPost("~/productcategories/create")]
//        public IActionResult Create(Productcategory model)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    int newCategoryId = _productCategoryData.InsertProductCategory(model.CategoryName, model.Description);
//                    ViewBag.Message = $"New Category ID: {newCategoryId}";
//                }
//                catch (Exception ex)
//                {
//                    ViewBag.Error = $"An error occurred: {ex.Message}";
//                }
//            }
//            return RedirectToAction("Index");
//        }

//        // Default MVC Edit (GET)
//        [HttpGet("~/productcategories/edit/{id}")]
//        public IActionResult Edit(int id)
//        {
//            var category = _productCategoryData.GetProductCategoryById(id);
//            if (category == null)
//            {
//                return NotFound();
//            }
//            return View(category);
//        }

//        // Default MVC Edit (POST)
//        [HttpPost("~/productcategories/edit/{id}")]
//        public IActionResult Edit(Productcategory model)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _productCategoryData.UpdateProductCategory(model.CategoryId, model.CategoryName, model.Description);
//                    ViewBag.Message = "Category updated successfully!";
//                }
//                catch (Exception ex)
//                {
//                    ViewBag.Error = $"An error occurred: {ex.Message}";
//                }
//            }
//            return View(model);
//        }

//        // Default MVC Delete (GET)
//        [HttpGet("~/productcategories/delete/{id}")]
//        public IActionResult Delete(int id)
//        {
//            var category = _productCategoryData.GetProductCategoryById(id);
//            if (category == null)
//            {
//                return NotFound();
//            }
//            return View(category);
//        }

//        // Default MVC Delete (POST)
//        [HttpPost("~/productcategories/delete/{id}"), ActionName("Delete")]
//        public IActionResult DeleteConfirmed(int id)
//        {
//            try
//            {
//                _productCategoryData.DeleteProductCategory(id);
//                ViewBag.Message = "Category deleted successfully!";
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Error = $"An error occurred: {ex.Message}";
//            }
//            return RedirectToAction("Index");
//        }
//    }
//}
