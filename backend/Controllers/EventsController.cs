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
    }
}
