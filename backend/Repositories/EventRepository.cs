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

        public async Task<List<Event>> GetAllApprovedAsync() =>
            await _events.Find(e => e.IsApproved && e.IsPublished).ToListAsync();

        public async Task<List<Event>> GetAllPendingAsync() =>
            await _events.Find(e => !e.IsApproved).ToListAsync();

        public async Task<List<Event>> GetByOrganizerIdAsync(string organizerId) =>
            await _events.Find(e => e.OrganizerId == organizerId).ToListAsync();

        public async Task<Event?> GetByIdAsync(string id) =>
            await _events.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Event ev) =>
            await _events.InsertOneAsync(ev);

        public async Task UpdateAsync(Event ev) =>
            await _events.ReplaceOneAsync(x => x.Id == ev.Id, ev);

        public async Task DeleteAsync(string id) =>
            await _events.DeleteOneAsync(e => e.Id == id);
    }
}
