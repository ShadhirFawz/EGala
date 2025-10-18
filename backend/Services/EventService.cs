using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class EventService
    {
        private readonly EventRepository _repo;

        public EventService(EventRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Event>> GetPublicEventsAsync(string? category = null, string? location = null, string? keyword = null)
            => await _repo.GetPublicEventsAsync(category, location, keyword);

        public async Task<List<Event>> GetOrganizerEventsAsync(string organizerId)
            => await _repo.GetByOrganizerAsync(organizerId);

        public async Task<Event?> GetByIdAsync(string id) => await _repo.GetByIdAsync(id);

        public async Task CreateEventAsync(Event ev) => await _repo.CreateAsync(ev);

        public async Task PublishEventAsync(string id)
        {
            var ev = await _repo.GetByIdAsync(id);
            if (ev == null) return;
            ev.IsPublished = true;
            await _repo.UpdateAsync(ev);
        }

        public async Task<bool> UpdateEventAsync(string id, Event updatedEvent)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            updatedEvent.Id = id;
            updatedEvent.OrganizerId = existing.OrganizerId; // ensure ownership
            updatedEvent.CreatedAt = existing.CreatedAt;
            updatedEvent.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(updatedEvent);
            return true;
        }

        public async Task<bool> DeleteEventAsync(string id, string? requesterId = null, bool isAdmin = false)
        {
            var ev = await _repo.GetByIdAsync(id);
            if (ev == null) return false;

            // Allow only owner or admin
            if (!isAdmin && ev.OrganizerId != requesterId) return false;

            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<List<Event>> GetNearbyEventsAsync(double latitude, double longitude, double radiusKm, string? category, string? keyword)
        {
            return await _repo.GetNearbyEventsAsync(latitude, longitude, radiusKm, category, keyword);
        }

        public async Task UpdateEventAsync(Event ev) => await _repo.UpdateAsync(ev);
    }
}
