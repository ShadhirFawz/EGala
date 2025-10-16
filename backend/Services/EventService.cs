using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class EventService(EventRepository repo)
    {
        private readonly EventRepository _repo = repo;

        public async Task<List<Event>> GetPublicEventsAsync()
            => await _repo.GetAllApprovedAsync();

        public async Task<List<Event>> GetOrganizerEventsAsync(string organizerId)
            => await _repo.GetByOrganizerIdAsync(organizerId);

        public async Task CreateEventAsync(Event ev)
            => await _repo.CreateAsync(ev);

        public async Task ApproveEventAsync(string id)
        {
            var ev = await _repo.GetByIdAsync(id) ?? throw new Exception("Event not found");
            ev.IsApproved = true;
            await _repo.UpdateAsync(ev);
        }

        public async Task<List<Event>> GetPendingEventsAsync()
            => await _repo.GetAllPendingAsync();

        public async Task PublishEventAsync(string id)
        {
            var ev = await _repo.GetByIdAsync(id);
            if (ev == null) throw new Exception("Event not found");
            if (!ev.IsApproved) throw new Exception("Event not approved by admin yet.");
            ev.IsPublished = true;
            await _repo.UpdateAsync(ev);
        }
    }
}
