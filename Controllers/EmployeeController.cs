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
            bool emplyeeExists = await _context.Employees.AnyAsync(u => u.Login == dto.Login);
            if (emplyeeExists)
            {
                return BadRequest("Користувач із таким Login вже зареєстрований.");
            }

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
        //СПИСОК УСІХ ПРАЦІВНИКІВ
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
        //ЗНАЙТИ ПРАЦІВНИКА ЗА ІД 
        [HttpGet("{id}")]
        public async Task<ActionResult> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if(employee == null)
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
        //ОНОВИТИ ТЕЛЕФОН ТА ПОСАДУ
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmployee(int id, EmployeeUpdateDTO dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if(employee == null)
            {
                return NotFound(new { message = "Працівника з таким ID не знайдено." });
            }
            bool phoneExists = await _context.Employees
                .AnyAsync(u => u.ContactPhone == dto.ContactPhone && u.EmployeeId != id);
            if(phoneExists)
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
