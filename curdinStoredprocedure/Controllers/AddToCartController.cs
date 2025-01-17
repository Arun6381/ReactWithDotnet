using curdinStoredprocedure.DataAccessLayer;
using curdinStoredprocedure.Filter;
using curdinStoredprocedure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace curdinStoredprocedure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddToCartController : ControllerBase
    {
        private readonly AddToartData _addToCartData;

        public AddToCartController(AddToartData addToCartData)
        {
            _addToCartData = addToCartData; 
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCartProduct([FromBody] AddToCartModel cart)
        {
            if (cart == null || cart.UserId <= 0 || cart.ProductId <= 0)
                return BadRequest("Invalid cart data.");

            try
            {
                var result = await _addToCartData.AddToCartProductAsync(cart.UserId, cart.ProductId);

                if (result == null)
                    return BadRequest("Failed to add product to cart.");

                return Ok(new { Message = "Product added to cart successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to cart: {ex.Message}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        //[AdminAuthorize]
        [HttpGet("GetCartDetail")]
        public async Task<IActionResult> GetCartDetailsWithInfo()
        {
            var cartDetails = await _addToCartData.GetCartDetailsWithInfoAsync();

            if (cartDetails == null || cartDetails.Count == 0)
                return NotFound("No cart items found.");

            return Ok(cartDetails);
        }


        [HttpGet("GetCartDetailsByUserId/{userId}")]
        public async Task<IActionResult> GetCartDetailsByUserId(int userId)
        {
            var cartDetails = await _addToCartData.GetCartDetailsByUserIdAsync(userId);

            if (cartDetails == null || cartDetails.Count == 0)
                return NotFound("No cart items found for this user.");

            return Ok(cartDetails);
        }


        [HttpPut("UpdateStatus/{cartId}")]
        public IActionResult UpdateStatusToCompleted(int cartId)
        {
            var result = _addToCartData.UpdateStatusToCompleted(cartId);

            if (result == null)
                return NotFound("Cart not found.");

            return Ok(new { Message = "Cart status updated to completed.", CartId = cartId });
        }


        [HttpDelete("DeleteCartItem/{cartId}")]
        public async Task<IActionResult> DeleteCartItem(int cartId)
        {
            var rowsAffected = await _addToCartData.DeleteCartItemAsync(cartId);

            if (rowsAffected > 0)
            {
                return Ok(new { Message = "Cart item deleted successfully.", CartId = cartId });
            }
            else
            {
                return NotFound(new { Message = "Cart item not found.", CartId = cartId });
            }
        }
    }
}
