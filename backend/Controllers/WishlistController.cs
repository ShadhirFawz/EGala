using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class WishlistController : ControllerBase
    {
        private readonly WishlistService _wishlistService;

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(ClaimTypes.Email)!;

            var result = await _wishlistService.GetWishlistAsync(userId);
            return Ok(result);
        }

        [HttpPost("add/{eventId}")]
        public async Task<IActionResult> AddToWishlist(string eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(ClaimTypes.Email)!;

            await _wishlistService.AddToWishlistAsync(userId, eventId);
            return Ok("Added to wishlist.");
        }

        [HttpDelete("remove/{eventId}")]
        public async Task<IActionResult> RemoveFromWishlist(string eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(ClaimTypes.Email)!;

            await _wishlistService.RemoveFromWishlistAsync(userId, eventId);
            return Ok("Removed from wishlist.");
        }
    }
}
