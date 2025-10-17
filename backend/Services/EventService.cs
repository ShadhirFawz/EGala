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

        public async Task UpdateEventAsync(Event ev) => await _repo.UpdateAsync(ev);
    }
}
