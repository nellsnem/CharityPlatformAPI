using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerController : ControllerBase
    {
        private readonly CharityContext _context;
        public VolunteerController(CharityContext context)
        {
            _context = context;
        }
        //стати волонтером
        [HttpPost]
        public async Task<ActionResult> CreateVolunteer(VolunteerCreateDTO dto)
        {
            bool userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.UserId);

            if(!userExists)
            {
                return NotFound("Користувача з таким ID не знайдено в системі.");
            }

            var existingVolunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == dto.UserId);

            if (existingVolunteer != null)
            {
                if (existingVolunteer.Status == "заблокований")
                {
                    return BadRequest("Вам назавжди заборонено бути волонтером через порушення правил.");
                }

                return BadRequest("Цей користувач вже є волонтером.");
            }

            var newVolunteer = new Volunteer
            {
                UserId = dto.UserId,
                Rating = 0,
                Status = "активний"

            };
            _context.Volunteers.Add(newVolunteer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Волонтера успішно створено!" });
        }
        //отримати список волонтерів
        [HttpGet]
        public async Task<ActionResult> GetAllVolunteers()
        {
            var list = await _context.Volunteers
                .Select(x => new VolunteerResponseDTO
                {
                     UserId = x.UserId,
                     VolunteerId = x.VolunteerId,
                     Rating =   x.Rating,
                    Status = x.Status

                }).ToListAsync();

            return Ok(list);
        }
        //знайит волонтера за ід
        [HttpGet("{id}")]
        public async Task<ActionResult> GetVolunteerById( int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);

            if(volunteer == null)
            {
                return NotFound("Волонтера з таким ID не знайдено.");
            }
            var responseDto = new VolunteerResponseDTO
            {
                UserId = volunteer.UserId,
                VolunteerId = volunteer.VolunteerId,
                Rating = volunteer.Rating,
                Status = volunteer.Status
            };

            return Ok(responseDto);
        }
        //оновити рейтинг
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateVolunteer(int id, VolunteerUpdateDTO dto)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);

            if (volunteer == null)
            {
                return NotFound("Волонтера з таким ID не знайдено.");
            }

            volunteer.Rating = dto.Rating;
            await _context.SaveChangesAsync();


            return Ok("Рейтинг волонтера успішно оновлено!");

        }
        //заблокувати волонтера
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVolonteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if(volunteer == null)
            {
                return NotFound("Волонтера з таким ID не знайдено.");
            }

            volunteer.Status = "заблокований";
            await _context.SaveChangesAsync();
            return Ok("Волонтера успішно заблоковано!");

        }
        // ПЕРЕВІРИТИ СТАТУС ВОЛОНТЕРА
        [HttpGet("check/{userId}")]
        public async Task<ActionResult> CheckVolunteerStatus(int userId)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);

            if (volunteer == null)
            {
                return Ok(new { isVolunteer = false, message = "Користувач не є волонтером." });
            }

            var responseDto = new VolunteerResponseDTO
            {
                VolunteerId = volunteer.VolunteerId,
                UserId = volunteer.UserId,
                Rating = volunteer.Rating,
                Status = volunteer.Status
            };

            return Ok(new { isVolunteer = true, volunteerData = responseDto });
        }
    }


}
