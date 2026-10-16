using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceEventsController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RaceEventsController(RacedayDbContext context)
        {
            _context = context;
        }
        private int GetCurrentOrganiserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out int id) ? id : 0;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _context.RaceEvents.ToListAsync();

            // Map list to DTO format
            var dtos = events.Select(e => new RaceEventDTO
            {
                EventID = e.EventID,
                Description = e.Description,
                Title = e.Title,
                location = e.location,
                OrganiserID = e.OrganiserID,
                CategoryID = e.CategoryID,
                EventType = e.EventType
            }).ToList();

            return Ok(dtos);
        }

        // 2. GET /api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _context.RaceEvents.FindAsync(id);
            if (e == null) return NotFound();

            var dto = new RaceEventDTO
            {
                EventID = e.EventID,
                Description = e.Description,
                Title = e.Title,
                location = e.location,
                OrganiserID = e.OrganiserID,
                CategoryID = e.CategoryID,
                EventType = e.EventType
            };

            return Ok(dto);
        }

        // 3. GET /api/events/mine
        [HttpGet("mine")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetMyEvents()
        {
            int organiserId = GetCurrentOrganiserId();
            var myEvents = await _context.RaceEvents
                .Where(e => e.OrganiserID == organiserId)
                .ToListAsync();

            var dtos = myEvents.Select(e => new RaceEventDTO
            {
                EventID = e.EventID,
                Description = e.Description,
                Title = e.Title,
                location = e.location,
                OrganiserID = e.OrganiserID,
                CategoryID = e.CategoryID,
                EventType = e.EventType
            }).ToList();

            return Ok(dtos);
        }

        // 4. POST /api/events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create(RaceEventDTO dto)
        {
            var newEvent = new RaceEvent
            {
                Title = dto.Title,
                Description = dto.Description,
                location = dto.location,
                CategoryID = dto.CategoryID,
                EventType = dto.EventType,
                OrganiserID = GetCurrentOrganiserId() // Overwrite with secure authenticated ID
            };

            _context.RaceEvents.Add(newEvent);
            await _context.SaveChangesAsync();

            dto.EventID = newEvent.EventID;
            dto.OrganiserID = newEvent.OrganiserID;

            return CreatedAtAction(nameof(GetById), new { id = newEvent.EventID }, dto);
        }

        // 5. PUT /api/events/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, RaceEventDTO dto)
        {
            var existingEvent = await _context.RaceEvents.FindAsync(id);
            if (existingEvent == null) return NotFound();

            // Enforce guard clause matching your guide requirements
            if (existingEvent.OrganiserID != GetCurrentOrganiserId()) return Forbid();

            existingEvent.Title = dto.Title;
            existingEvent.Description = dto.Description;
            existingEvent.location = dto.location;
            existingEvent.CategoryID = dto.CategoryID;
            existingEvent.EventType = dto.EventType;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6. DELETE /api/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingEvent = await _context.RaceEvents.FindAsync(id);
            if (existingEvent == null) return NotFound();

            if (existingEvent.OrganiserID != GetCurrentOrganiserId()) return Forbid();

            _context.RaceEvents.Remove(existingEvent);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
