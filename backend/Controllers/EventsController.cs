using backend.Models;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Security.Claims;

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

        // ORGANIZER: Create event
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
                Address = dto.Address,
                Location = new GeoJsonPoint<GeoJson2DCoordinates>(
                    new GeoJson2DCoordinates(dto.Longitude, dto.Latitude)
                ),
                Date = dto.Date,
                TicketPackages = dto.TicketPackages.Select(p => new TicketPackage
                {
                    Name = p.Name,
                    Price = p.Price,
                    Capacity = p.Capacity,
                    Materials = p.Materials
                }).ToList(),
                SeatLayout = dto.SeatLayout?.Select(s => new Seat { Row = s.Row, Col = s.Col }).ToList(),
                Images = dto.Images ?? new List<string>(),
                TotalCapacity = dto.TicketPackages.Sum(p => p.Capacity),
                OrganizerId = organizerId,
                IsPublished = dto.IsPublished
            };

            await _eventService.CreateEventAsync(newEvent);
            return Ok("Event created successfully!");
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

        // ORGANIZER: Publish event
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
                Address = dto.Address,
                Location = new MongoDB.Driver.GeoJsonObjectModel.GeoJsonPoint<MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates>(
                    new MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates(dto.Longitude, dto.Latitude)
                ),
                Date = dto.Date,
                TicketPackages = dto.TicketPackages.Select(p => new TicketPackage
                {
                    Name = p.Name,
                    Price = p.Price,
                    Capacity = p.Capacity,
                    Materials = p.Materials
                }).ToList(),
                SeatLayout = dto.SeatLayout?.Select(s => new Seat { Row = s.Row, Col = s.Col }).ToList(),
                Images = dto.Images ?? new List<string>(),
                TotalCapacity = dto.TicketPackages.Sum(p => p.Capacity),
                OrganizerId = organizerId,
                IsPublished = dto.IsPublished
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

        [HttpPost("nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearbyEvents([FromBody] NearbySearchDTO dto)
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
