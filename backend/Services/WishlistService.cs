using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class WishlistService
    {
        private readonly WishlistRepository _repo;

        public WishlistService(WishlistRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<string>> GetWishlistAsync(string userId)
        {
            var wishlist = await _repo.GetByUserIdAsync(userId);
            return wishlist?.EventIds ?? new List<string>();
        }

        public async Task AddToWishlistAsync(string userId, string eventId)
        {
            var wishlist = await _repo.GetByUserIdAsync(userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId, EventIds = new List<string> { eventId } };
                await _repo.CreateAsync(wishlist);
            }
            else if (!wishlist.EventIds.Contains(eventId))
            {
                wishlist.EventIds.Add(eventId);
                await _repo.UpdateAsync(wishlist);
            }
        }

        public async Task RemoveFromWishlistAsync(string userId, string eventId)
        {
            var wishlist = await _repo.GetByUserIdAsync(userId);
            if (wishlist == null) return;

            wishlist.EventIds.Remove(eventId);
            await _repo.UpdateAsync(wishlist);
        }
    }
}
