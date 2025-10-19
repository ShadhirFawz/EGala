using backend.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.GeoJsonObjectModel;

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

            // Create geospatial index for locations.coordinates (array field)
            // Use string path because Locations is a nested array
            var indexKeys = Builders<Event>.IndexKeys.Geo2DSphere("locations.coordinates");
            _events.Indexes.CreateOne(new CreateIndexModel<Event>(indexKeys));
        }

        public async Task<List<Event>> GetPublicEventsAsync(string? category = null, string? locationName = null, string? keyword = null)
        {
            var filterBuilder = Builders<Event>.Filter;
            var filter = filterBuilder.Eq(e => e.IsPublished, true);

            if (!string.IsNullOrEmpty(category))
                filter &= filterBuilder.Eq(e => e.Category, category);

            if (!string.IsNullOrEmpty(locationName))
                // match by location name inside Locations array
                filter &= filterBuilder.ElemMatch(e => e.Locations, Builders<EventLocation>.Filter.Eq(l => l.Name, locationName));

            if (!string.IsNullOrEmpty(keyword))
                filter &= filterBuilder.Or(
                    filterBuilder.Regex(e => e.Title, new BsonRegularExpression(keyword, "i")),
                    filterBuilder.Regex(e => e.Description, new BsonRegularExpression(keyword, "i"))
                );

            return await _events.Find(filter).ToListAsync();
        }

        public async Task<List<Event>> GetNearbyEventsAsync(double latitude, double longitude, double radiusKm, string? category = null, string? keyword = null)
        {
            var point = new GeoJsonPoint<GeoJson2DCoordinates>(new GeoJson2DCoordinates(longitude, latitude));

            var filterBuilder = Builders<Event>.Filter;

            // Geo filter using $geoWithin for nested locations array
            var geoFilter = new BsonDocument("$geoWithin",
                new BsonDocument("$centerSphere",
                    new BsonArray { new BsonArray { longitude, latitude }, radiusKm / 6378.1 }));

            // Base event filter
            var baseFilter = filterBuilder.Eq(e => e.IsPublished, true);

            if (!string.IsNullOrEmpty(category))
                baseFilter &= filterBuilder.Eq(e => e.Category, category);

            if (!string.IsNullOrEmpty(keyword))
                baseFilter &= filterBuilder.Or(
                    filterBuilder.Regex(e => e.Title, new MongoDB.Bson.BsonRegularExpression(keyword, "i")),
                    filterBuilder.Regex(e => e.Description, new MongoDB.Bson.BsonRegularExpression(keyword, "i"))
                );

            // Convert to BsonDocument safely using RenderArgs
            var args = new RenderArgs<Event>(
                BsonSerializer.SerializerRegistry.GetSerializer<Event>(),
                BsonSerializer.SerializerRegistry
            );

            var combinedFilter = baseFilter.Render(args);

            var rawFilter = new BsonDocument
            {
                {
                    "$and", new BsonArray
                    {
                        combinedFilter,
                        new BsonDocument("locations.coordinates", geoFilter)
                    }
                }
            };

            return await _events.Find(rawFilter).ToListAsync();
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
