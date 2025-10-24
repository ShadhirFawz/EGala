using backend.Models;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        // PUBLIC: Browse with filters
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicEvents(
            [FromQuery] string? category,
            [FromQuery] string? location,
            [FromQuery] string? keyword)
        {
            var events = await _eventService.GetPublicEventsAsync(category, location, keyword);
            return Ok(events);
        }

        // ORGANIZER: Create event (organizer does NOT add ticket packages)
        [HttpPost("create")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDTO dto)
        {
            var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                               ?? User.FindFirstValue(ClaimTypes.Email)!;

            var newEvent = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                OrganizerId = organizerId,
                OrganizingFee = dto.OrganizingFee,
                Locations = dto.Locations.Select(l => new EventLocation
                {
                    Name = l.Name,
                    Coordinates = new GeoJsonPoint<GeoJson2DCoordinates>(
                        new GeoJson2DCoordinates(l.Longitude, l.Latitude)
                    )
                }).ToList(),
                AvailableDates = dto.AvailableDates,
                SeatMatrices = dto.SeatMatrices.Select(s => new SeatMatrix
                {
                    SeatType = s.SeatType,
                    Rows = s.Rows,
                    Cols = s.Cols,
                    PricePerSeat = s.PricePerSeat
                }).ToList(),
                Images = dto.Images ?? new System.Collections.Generic.List<string>(),
                IsApproved = false,
                IsPublished = false
            };

            await _eventService.CreateEventAsync(newEvent);
            return Ok("Event created successfully and awaiting approval!");
        }

        // PUBLIC: Get event details by ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(string id)
        {
            var ev = await _eventService.GetEventByIdAsync(id);
            if (ev == null)
                return NotFound("Event not found or not published.");
            return Ok(ev);
        }

        // ORGANIZER: View own events
        [HttpGet("mine")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> GetMyEvents()
        {
            var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                               ?? User.FindFirstValue(ClaimTypes.Email)!;

            var events = await _eventService.GetOrganizerEventsAsync(organizerId);
            return Ok(events);
        }

        // ADMIN: View all pending (not yet approved) events
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingEvents()
        {
            var all = await _eventService.GetAllEventsAsync();
            var pending = all.Where(e => !e.IsApproved).ToList();
            return Ok(pending);
        }

        // ADMIN: Approve an event
        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveEvent(string id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null) return NotFound("Event not found.");

            ev.IsApproved = true;
            await _eventService.UpdateEventAsync(ev);
            return Ok("Event approved successfully!");
        }

        // ORGANIZER: Publish event (after admin approval — admin flow not implemented here)
        [HttpPut("publish/{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> PublishEvent(string id)
        {
            await _eventService.PublishEventAsync(id);
            return Ok("Event published successfully!");
        }

        // ORGANIZER: Update event details
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> UpdateEvent(string id, [FromBody] EventDTO dto)
        {
            var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? User.FindFirstValue(ClaimTypes.Email)!;

            var updatedEvent = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                OrganizerId = organizerId,
                OrganizingFee = dto.OrganizingFee,
                Locations = dto.Locations.Select(l => new EventLocation
                {
                    Name = l.Name,
                    Coordinates = new GeoJsonPoint<GeoJson2DCoordinates>(
                        new GeoJson2DCoordinates(l.Longitude, l.Latitude)
                    )
                }).ToList(),
                AvailableDates = dto.AvailableDates,
                SeatMatrices = dto.SeatMatrices.Select(s => new SeatMatrix
                {
                    SeatType = s.SeatType,
                    Rows = s.Rows,
                    Cols = s.Cols,
                    PricePerSeat = s.PricePerSeat
                }).ToList(),
                Images = dto.Images ?? new System.Collections.Generic.List<string>(),
                IsPublished = false // when updated, keep unpublished until approved/published explicitly
            };

            var result = await _eventService.UpdateEventAsync(id, updatedEvent);
            if (!result) return NotFound("Event not found.");

            return Ok("Event updated successfully!");
        }

        // ORGANIZER or ADMIN: Delete event
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            var isAdmin = User.IsInRole("Admin");
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? User.FindFirstValue(ClaimTypes.Email)!;

            var success = await _eventService.DeleteEventAsync(id, requesterId, isAdmin);
            if (!success) return Forbid("You are not authorized or event not found.");

            return Ok("Event deleted successfully.");
        }

        // NEARBY: find events around a point (by location coordinates inside locations array)
        [HttpPost("nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearbyEvents([FromBody] DTOs.NearbySearchDTO dto)
        {
            var events = await _eventService.GetNearbyEventsAsync(
                dto.Latitude,
                dto.Longitude,
                dto.RadiusKm,
                dto.Category,
                dto.Keyword
            );

            if (events.Count == 0)
                return NotFound("No events found nearby.");

            return Ok(events);
        }
    }
}
