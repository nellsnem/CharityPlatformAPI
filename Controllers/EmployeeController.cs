using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly CharityContext _context;

        public EmployeeController(CharityContext context)
        {
            _context = context;
        }

        // СТВОРЕННЯ ПРАЦІВНИКА
        [HttpPost]
        public async Task<ActionResult> CreateEmployee(EmployeeCreateDTO dto)
        {
            try
            {
                // 1. Перевірка на дублікат логіна
                bool employeeExists = await _context.Employees.AnyAsync(u => u.Login == dto.Login);
                if (employeeExists)
                {
                    return BadRequest("Користувач із таким Login вже зареєстрований.");
                }

                // 2. Перевірка на дублікат телефону (ТЕ, ЧОГО НЕ ВИСТАЧАЛО)
                bool phoneExists = await _context.Employees.AnyAsync(u => u.ContactPhone == dto.ContactPhone);
                if (phoneExists)
                {
                    return BadRequest("Працівник із таким номером телефону вже зареєстрований.");
                }

                // 3. Збереження
                var newEmployee = new Employee
                {
                    FullName = dto.FullName,
                    Position = dto.Position,
                    ContactPhone = dto.ContactPhone,
                    Login = dto.Login,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };

                _context.Employees.Add(newEmployee);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Працівника успішно створено!" });
            }
            catch (Exception ex)
            {
                // Справжній перехват будь-яких системних помилок БД
                return StatusCode(500, $"Внутрішня помилка сервера: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // ВХІД ПРАЦІВНИКА (АВТОРИЗАЦІЯ)
        [HttpPost("login")]
        public async Task<ActionResult> LoginEmployee(EmployeeLoginDTO dto)
        {
            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Login == dto.Login);

                if (employee == null)
                {
                    return BadRequest("Невірний логін або пароль.");
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, employee.PasswordHash);

                if (!isPasswordValid)
                {
                    return BadRequest("Невірний логін або пароль.");
                }

                return Ok(new
                {
                    employeeId = employee.EmployeeId,
                    fullName = employee.FullName,
                    position = employee.Position
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Внутрішня помилка сервера при спробі входу.");
            }
        }

        // СПИСОК УСІХ ПРАЦІВНИКІВ
        [HttpGet]
        public async Task<ActionResult> GetAllEmployees()
        {
            var list = await _context.Employees
                .Select(x => new EmployeeResponseDTO
                {
                    EmployeeId = x.EmployeeId,
                    FullName = x.FullName,
                    Position = x.Position,
                    ContactPhone = x.ContactPhone,
                    Login = x.Login
                }).ToListAsync();

            return Ok(list);
        }

        // ЗНАЙТИ ПРАЦІВНИКА ЗА ІД 
        [HttpGet("{id}")]
        public async Task<ActionResult> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Працівника з таким ID не знайдено.");
            }

            var responseDto = new EmployeeResponseDTO
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Position = employee.Position,
                ContactPhone = employee.ContactPhone,
                Login = employee.Login
            };

            return Ok(responseDto);
        }

        // ОНОВИТИ ТЕЛЕФОН ТА ПОСАДУ
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmployee(int id, EmployeeUpdateDTO dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Працівника з таким ID не знайдено." });
            }

            bool phoneExists = await _context.Employees
                .AnyAsync(u => u.ContactPhone == dto.ContactPhone && u.EmployeeId != id);
            if (phoneExists)
            {
                return BadRequest(new { message = "Цей номер телефону вже зайнятий іншим працівником." });
            }

            employee.ContactPhone = dto.ContactPhone;
            employee.Position = dto.Position;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Дані працівника успішно оновлено!" });
        }
    }
}