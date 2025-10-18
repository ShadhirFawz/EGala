using backend.Models;
using MongoDB.Driver;

namespace backend.Repositories
{
    public class WishlistRepository
    {
        private readonly IMongoCollection<Wishlist> _wishlist;

        public WishlistRepository(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
            var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
            _wishlist = db.GetCollection<Wishlist>("wishlists");
        }

        public async Task<Wishlist?> GetByUserIdAsync(string userId)
            => await _wishlist.Find(w => w.UserId == userId).FirstOrDefaultAsync();

        public async Task CreateAsync(Wishlist wishlist)
            => await _wishlist.InsertOneAsync(wishlist);

        public async Task UpdateAsync(Wishlist wishlist)
            => await _wishlist.ReplaceOneAsync(w => w.Id == wishlist.Id, wishlist);
    }
}
