using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly CharityContext _context;

        public SupplierController(CharityContext context)
        {
            _context = context;
        }

        // СТВОРИТИ ПОСТАЧАЛЬНИКА
        [HttpPost]
        public async Task<ActionResult> CreateSupplier(SupplierCreateDTO dto)
        {
            bool edrpouExists = await _context.Suppliers.AnyAsync(s => s.EdrpouCode == dto.EdrpouCode);
            if (edrpouExists)
            {
                return BadRequest(new { message = "Постачальник з таким кодом ЄДРПОУ вже існує в базі." });
            }

            var newSupplier = new Supplier
            {
                CompanyName = dto.CompanyName,
                EdrpouCode = dto.EdrpouCode,
                Contacts = dto.Contacts
            };

            _context.Suppliers.Add(newSupplier);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Постачальника успішно створено!" });
        }

        // ЗНАЙТИ ВСІХ ПОСТАЧАЛЬНИКІВ
        [HttpGet]
        public async Task<ActionResult> GetAllSuppliers()
        {
            var list = await _context.Suppliers
                .Select(s => new SupplierResponseDTO
                {
                    SupplierId = s.SupplierId,
                    CompanyName = s.CompanyName,
                    EdrpouCode = s.EdrpouCode,
                    Contacts = s.Contacts
                }).ToListAsync();

            return Ok(list);
        }

        // ЗНАЙТИ ПОСТАЧАЛЬНИКА ЗА ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSupplierById(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = "Постачальника з таким ID не знайдено." });
            }

            var responseDto = new SupplierResponseDTO
            {
                SupplierId = supplier.SupplierId,
                CompanyName = supplier.CompanyName,
                EdrpouCode = supplier.EdrpouCode,
                Contacts = supplier.Contacts
            };

            return Ok(responseDto);
        }

        // ОНОВИТИ ДАНІ ПОСТАЧАЛЬНИКА
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSupplier(int id, SupplierUpdateDTO dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = "Постачальника з таким ID не знайдено." });
            }

            if (supplier.EdrpouCode != dto.EdrpouCode)
            {
                bool edrpouExists = await _context.Suppliers.AnyAsync(s => s.EdrpouCode == dto.EdrpouCode);
                if (edrpouExists)
                {
                    return BadRequest(new { message = "Цей код ЄДРПОУ вже зареєстрований для іншої компанії." });
                }
            }

            supplier.CompanyName = dto.CompanyName;
            supplier.EdrpouCode = dto.EdrpouCode;
            supplier.Contacts = dto.Contacts;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Дані постачальника успішно оновлено!" });
        }
    }
}