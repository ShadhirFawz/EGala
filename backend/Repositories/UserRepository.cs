using backend.Models;
using MongoDB.Driver;

namespace backend.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
            _users = database.GetCollection<User>("users");
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<User?> GetByGoogleIdAsync(string googleId) =>
            await _users.Find(u => u.GoogleId == googleId).FirstOrDefaultAsync();

        public async Task<User?> GetByIdAsync(string id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(User user) =>
            await _users.InsertOneAsync(user);

        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }
    }
}
