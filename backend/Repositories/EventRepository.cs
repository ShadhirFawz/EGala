using backend.Models;
using MongoDB.Driver;

namespace backend.Repositories
{
    public class EventRepository
    {
        private readonly IMongoCollection<Event> _events;

        public EventRepository(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
            var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
            _events = db.GetCollection<Event>("events");
        }

        public async Task<List<Event>> GetPublicEventsAsync(string? category = null, string? location = null, string? keyword = null)
        {
            var filterBuilder = Builders<Event>.Filter;
            var filter = filterBuilder.Eq(e => e.IsPublished, true);

            if (!string.IsNullOrEmpty(category))
                filter &= filterBuilder.Eq(e => e.Category, category);

            if (!string.IsNullOrEmpty(location))
                filter &= filterBuilder.AnyEq(e => e.Locations, location);

            if (!string.IsNullOrEmpty(keyword))
                filter &= filterBuilder.Or(
                    filterBuilder.Regex(e => e.Title, new MongoDB.Bson.BsonRegularExpression(keyword, "i")),
                    filterBuilder.Regex(e => e.Description, new MongoDB.Bson.BsonRegularExpression(keyword, "i"))
                );

            return await _events.Find(filter).ToListAsync();
        }

        public async Task<List<Event>> GetByOrganizerAsync(string organizerId) =>
            await _events.Find(e => e.OrganizerId == organizerId).ToListAsync();

        public async Task<Event?> GetByIdAsync(string id) =>
            await _events.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Event ev) => await _events.InsertOneAsync(ev);

        public async Task UpdateAsync(Event ev)
        {
            ev.UpdatedAt = DateTime.UtcNow;
            await _events.ReplaceOneAsync(x => x.Id == ev.Id, ev);
        }

        public async Task DeleteAsync(string id) =>
            await _events.DeleteOneAsync(e => e.Id == id);
    }
}
