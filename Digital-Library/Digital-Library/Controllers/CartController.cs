using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Digital_Library.Controllers
{
    [Route("Cart")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var cartItems = await cartService.GetCartAsync(userId);
            return View(cartItems.Data as Cart);
        }

        //[HttpPost("AddItem")]
        //public async Task<IActionResult> AddItem([FromBody] CartDetailRequest request)
        //{
        //	var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //	if (userId == null) return Unauthorized();

        //	var result = await cartService.AddItemAsync(userId, request);
        //	return Json(result);
        //}
        //new one when add in book details
        [HttpPost("AddItem")]
        [Authorize]
        public async Task<IActionResult> AddItem([FromBody] CartDetailRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var result = await cartService.AddItemAsync(userId, request);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = "Book added to cart successfully!" });
            }
            catch (Exception ex)
            {
                // log ex
                return StatusCode(500, new { success = false, message = "Server error: " + ex.Message });
            }
        }


        [HttpGet("Remove")]
        public async Task<IActionResult> Remove(string cartDetailId)
        {
            if (string.IsNullOrEmpty(cartDetailId))
                return BadRequest("Invalid item id");

            var result = await cartService.RemoveItemAsync(cartDetailId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromForm] string cartDetailId, [FromForm] int quantity)
        {
            if (string.IsNullOrEmpty(cartDetailId))
                return BadRequest("Invalid item id");
            var result = await cartService.UpdateCartAsync(cartDetailId, quantity);
            return Json(result);
        }

        [HttpPost("Clear")]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await cartService.ClearCartAsync(userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
