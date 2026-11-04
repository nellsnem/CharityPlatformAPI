using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerParticipationController : ControllerBase
    {
        private readonly CharityContext _context;

        public VolunteerParticipationController(CharityContext context)
        {
            _context = context;
        }

        // СТВОРИТИ УЧАСТЬ ВОЛОНТЕРА
        [HttpPost]
        public async Task<ActionResult> CreateParticipation(ParticipationCreateDTO dto)
        {
            var newParticipation = new VolunteerParticipation
            {
                EventId = dto.EventId,
                VolunteerId = dto.VolunteerId,
                EmployeeId = dto.EmployeeId,
                StatusPresence = "записався",
                HoursWorked = 0
            };

            _context.VolunteerParticipations.Add(newParticipation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Волонтера успішно зареєстровано на захід!" });
        }

        // ЗНАЙТИ ВСІ ЗАПИСИ
        [HttpGet]
        public async Task<ActionResult> GetAllParticipations()
        {
            var list = await _context.VolunteerParticipations
                .Select(x => new ParticipationResponseDTO
                {
                    ParticipationId = x.ParticipationId,
                    EventId = x.EventId,
                    VolunteerId = x.VolunteerId,
                    EmployeeId = x.EmployeeId,
                    StatusPresence = x.StatusPresence,
                    HoursWorked = x.HoursWorked
                }).ToListAsync();

            return Ok(list);
        }

        // ЗНАЙТИ УЧАСТЬ ЗА АЙДІ
        [HttpGet("{id}")]
        public async Task<ActionResult> GetParticipationById(int id)
        {
            var p = await _context.VolunteerParticipations.FindAsync(id);

            if (p == null)
            {
                return NotFound(new { message = "Запис про участь не знайдено." });
            }

            var responseDto = new ParticipationResponseDTO
            {
                ParticipationId = p.ParticipationId,
                EventId = p.EventId,
                VolunteerId = p.VolunteerId,
                EmployeeId = p.EmployeeId,
                StatusPresence = p.StatusPresence,
                HoursWorked = p.HoursWorked
            };

            return Ok(responseDto);
        }

        // ЗНАЙТИ ВОЛОНТЕРІВ ЗА АЙДІ ЗАХОДУ
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult> GetParticipationsByEventId(int eventId)
        {
            var participations = await _context.VolunteerParticipations
                .Where(p => p.EventId == eventId)
                .Select(x => new ParticipationResponseDTO
                {
                    ParticipationId = x.ParticipationId,
                    EventId = x.EventId,
                    VolunteerId = x.VolunteerId,
                    EmployeeId = x.EmployeeId,
                    StatusPresence = x.StatusPresence,
                    HoursWorked = x.HoursWorked
                }).ToListAsync();

            if (participations.Count == 0)
            {
                return NotFound(new { message = "На цей захід ще не зареєстровано жодного волонтера." });
            }

            return Ok(participations);
        }

        // ОНОВИТИ СТАТУС ТА ГОДИНИ
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateParticipation(int id, ParticipationUpdateDTO dto)
        {
            var p = await _context.VolunteerParticipations.FindAsync(id);
            if (p == null)
            {
                return NotFound(new { message = "Запис про участь не знайдено." });
            }

            p.StatusPresence = dto.StatusPresence;
            p.HoursWorked = dto.HoursWorked;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Дані про участь успішно оновлено!" });
        }
    }
}