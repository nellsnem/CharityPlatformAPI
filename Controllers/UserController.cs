using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CharityContext _context;

        public UserController(CharityContext context)
        {
            _context = context;
        }

        // ДОДАТИ КЛІЄНТА (Реєстрація)
        [HttpPost("register")]
        public async Task<ActionResult> CreateUser(UserAccountCreateDTO dto)
        {
            bool phoneExists = await _context.UserAccounts.AnyAsync(u => u.Phone == dto.Phone);

            if (phoneExists)
            {
                // Повертаємо JSON замість простого тексту
                return BadRequest(new { message = "Користувач із таким номером телефону вже зареєстрований." });
            }
            var newUser = new UserAccount
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.UserAccounts.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Користувача успішно створено!" });
        }

        // ВХІД
        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDTO dto)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Phone == dto.Phone);

            if (user == null)
            {
                return BadRequest(new { message = "Невірний номер телефону або пароль." });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return BadRequest(new { message = "Невірний номер телефону або пароль." });
            }

            // ДОДАНО fullName, щоб фронтенд міг показати його в шапці
            return Ok(new { message = "Успішний вхід у систему!", userId = user.UserId, fullName = user.FullName });
        }

        // подивитись на клієнта
        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            var list = await _context.UserAccounts
                .Select(x => new UserAccountResponseDTO
                {
                    UserId = x.UserId,
                    FullName = x.FullName,
                    Phone = x.Phone,
                }).ToListAsync();

            return Ok(list);
        }

        // пошук користувача за ід
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _context.UserAccounts.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Користувача з таким ID не знайдено." });
            }
            var responseDto = new UserAccountResponseDTO
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Phone = user.Phone,
            };
            return Ok(responseDto);
        }

        // змінити дані користувача
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UserAccountUpdateDTO dto)
        {
            var user = await _context.UserAccounts.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Користувача з таким ID не знайдено." });
            }

            bool phoneExists = await _context.UserAccounts
                .AnyAsync(u => u.Phone == dto.Phone && u.UserId != id);

            if (phoneExists)
            {
                return BadRequest(new { message = "Цей номер телефону вже зайнятий іншим користувачем." });
            }
            user.FullName = dto.FullName;
            user.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Дані користувача успішно оновлено!" });
        }

        // видалити користувача
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.UserAccounts.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Користувача з таким ID не знайдено." });
            }

            bool isVolunteer = await _context.Volunteers.AnyAsync(v => v.UserId == id);
            bool isBeneficiary = await _context.Beneficiaries.AnyAsync(b => b.UserId == id);

            if (isVolunteer || isBeneficiary)
            {
                return BadRequest(new { message = "Цього користувача не можна видалити, оскільки він є волонтером або бенефіціаром. Будь ласка, заблокуйте його обліковий запис у відповідному розділі." });
            }

            _context.UserAccounts.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Дані користувача успішно видалено!" });
        }
    }
}