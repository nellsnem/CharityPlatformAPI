using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
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
        //додати клієнта
        [HttpPost]
        public async Task<ActionResult> CreateUser(UserAccountCreateDTO dto)
        {
            bool phoneExists = await _context.UserAccounts.AnyAsync(u => u.Phone == dto.Phone);

            if (phoneExists)
            {
                return BadRequest("Користувач із таким номером телефону вже зареєстрований.");
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

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDTO dto)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Phone == dto.Phone);

            if (user == null)
            {
                return BadRequest("Невірний номер телефону або пароль.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return BadRequest("Невірний номер телефону або пароль.");
            }
            return Ok(new { message = "Успішний вхід у систему!", userId = user.UserId });
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

            if(user == null)
            {
                return NotFound("Користувача з таким ID не знайдено.");
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
                return NotFound("Користувача з таким ID не знайдено.");

            }

            bool phoneExists = await _context.UserAccounts
                .AnyAsync(u => u.Phone == dto.Phone && u.UserId != id);
            if (phoneExists)
            {
                return BadRequest("Цей номер телефону вже зайнятий іншим користувачем.");
            }
            user.FullName = dto.FullName;
            user.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return Ok("Дані користувача успішно оновлено!");
        }
        // видалити користувача
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.UserAccounts.FindAsync(id);
            if (user == null)
            {
                return NotFound("Користувача з таким ID не знайдено.");
            }

            _context.UserAccounts.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("Дані користувача успішно видалено!");
        }
        
    }
}