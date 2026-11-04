using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly CharityContext _context;

        public EventController(CharityContext context)
        {
            _context = context;
        }

        // СТВОРИТИ ЗАХІД
        [HttpPost]
        public async Task<ActionResult> CreateEvent(EventCreateDTO dto)
        {
            var newEvent = new Event
            {
                ProjectId = dto.ProjectId,
                EmployeeId = dto.EmployeeId,
                Title = dto.Title,
                EventDate = dto.EventDate.ToUniversalTime(),
                Location = dto.Location,
                Description = dto.Description
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Захід успішно створено!" });
        }

        // ЗНАЙТИ ВСІ ЗАХОДИ
        [HttpGet]
        public async Task<ActionResult> GetAllEvents()
        {
            var list = await _context.Events
                .Select(x => new EventResponseDTO
                {
                    EventId = x.EventId,
                    ProjectId = x.ProjectId,
                    EmployeeId = x.EmployeeId,
                    Title = x.Title,
                    EventDate = x.EventDate,
                    Location = x.Location,
                    Description = x.Description
                }).ToListAsync();

            return Ok(list);
        }

        // ЗНАЙТИ ЗАХІД ЗА АЙДІ
        [HttpGet("{id}")]
        public async Task<ActionResult> GetEventById(int id)
        {
            var ev = await _context.Events.FindAsync(id);

            if (ev == null)
            {
                return NotFound(new { message = "Захід з таким ID не знайдено." });
            }

            var responseDto = new EventResponseDTO
            {
                EventId = ev.EventId,
                ProjectId = ev.ProjectId,
                EmployeeId = ev.EmployeeId,
                Title = ev.Title,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Description = ev.Description
            };

            return Ok(responseDto);
        }

        // ЗНАЙТИ ЗАХОДИ ЗА АЙДІ ПРОЄКТУ
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult> GetEventsByProjectId(int projectId)
        {
            var events = await _context.Events
                .Where(e => e.ProjectId == projectId)
                .Select(x => new EventResponseDTO
                {
                    EventId = x.EventId,
                    ProjectId = x.ProjectId,
                    EmployeeId = x.EmployeeId,
                    Title = x.Title,
                    EventDate = x.EventDate,
                    Location = x.Location,
                    Description = x.Description
                }).ToListAsync();

            if (events.Count == 0)
            {
                return NotFound(new { message = "Заходів для цього проєкту не знайдено." });
            }

            return Ok(events);
        }

        // ОНОВИТИ ЗАХІД (назву, час, місце або опис)
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEvent(int id, EventUpdateDTO dto)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
            {
                return NotFound(new { message = "Захід з таким ID не знайдено." });
            }

            ev.Title = dto.Title;
            ev.EventDate = dto.EventDate.ToUniversalTime();
            ev.Location = dto.Location;
            ev.Description = dto.Description;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Дані заходу успішно оновлено!" });
        }
    }
}

