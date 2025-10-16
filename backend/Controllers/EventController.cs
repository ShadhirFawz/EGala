using backend.Models;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController(EventService eventService) : ControllerBase
    {
        private readonly EventService _eventService = eventService;

        // PUBLIC: List approved & published events
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicEvents()
        {
            var events = await _eventService.GetPublicEventsAsync();
            return Ok(events);
        }

        // ORGANIZER: Create new event
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
                Date = dto.Date,
                Location = dto.Location,
                Price = dto.Price,
                AvailableSeats = dto.AvailableSeats,
                OrganizerId = organizerId
            };

            await _eventService.CreateEventAsync(newEvent);
            return Ok("Event created successfully (awaiting admin approval).");
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

        // ORGANIZER: Publish after admin approval
        [HttpPut("publish/{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> PublishEvent(string id)
        {
            await _eventService.PublishEventAsync(id);
            return Ok("Event published successfully!");
        }

        // ADMIN: View all pending events
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingEvents()
        {
            var events = await _eventService.GetPendingEventsAsync();
            return Ok(events);
        }

        // ADMIN: Approve event
        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveEvent(string id)
        {
            await _eventService.ApproveEventAsync(id);
            return Ok("Event approved successfully!");
        }
    }
}
