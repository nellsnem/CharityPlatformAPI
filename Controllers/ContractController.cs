using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly CharityContext _context;

        public ContractController(CharityContext context)
        {
            _context = context;
        }

        // СТВОРИТИ ДОГОВІР
        [HttpPost]
        public async Task<ActionResult> CreateContract(ContractCreateDTO dto)
        {
            bool supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierId == dto.SupplierId);
            if (!supplierExists)
            {
                return NotFound(new { message = "Постачальника з таким ID не знайдено." });
            }

            bool projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == dto.ProjectId);
            if (!projectExists)
            {
                return NotFound(new { message = "Проєкт з таким ID не знайдено." });
            }

            var newContract = new Contract
            {
                SupplierId = dto.SupplierId,
                ProjectId = dto.ProjectId,
                SigningDate = dto.SigningDate,
                Subject = dto.Subject,
                Quantity = dto.Quantity,
                Amount = dto.Amount,
                Status = "чинний" 
            };

            _context.Contracts.Add(newContract);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Договір успішно створено!" });
        }

        // ЗНАЙТИ ВСІ ДОГОВОРИ
        [HttpGet]
        public async Task<ActionResult> GetAllContracts()
        {
            var list = await _context.Contracts
                .Select(c => new ContractResponseDTO
                {
                    ContractId = c.ContractId,
                    SupplierId = c.SupplierId,
                    ProjectId = c.ProjectId,
                    SigningDate = c.SigningDate,
                    Subject = c.Subject,
                    Quantity = c.Quantity,
                    Amount = c.Amount,
                    Status = c.Status
                }).ToListAsync();

            return Ok(list);
        }

        // ЗНАЙТИ ДОГОВІР ЗА ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetContractById(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound(new { message = "Договір з таким ID не знайдено." });
            }

            var responseDto = new ContractResponseDTO
            {
                ContractId = contract.ContractId,
                SupplierId = contract.SupplierId,
                ProjectId = contract.ProjectId,
                SigningDate = contract.SigningDate,
                Subject = contract.Subject,
                Quantity = contract.Quantity,
                Amount = contract.Amount,
                Status = contract.Status
            };

            return Ok(responseDto);
        }

        // ЗНАЙТИ ДОГОВОРИ ЗА ID ПРОЄКТУ
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult> GetContractsByProjectId(int projectId)
        {
            var contracts = await _context.Contracts
                .Where(c => c.ProjectId == projectId)
                .Select(c => new ContractResponseDTO
                {
                    ContractId = c.ContractId,
                    SupplierId = c.SupplierId,
                    ProjectId = c.ProjectId,
                    SigningDate = c.SigningDate,
                    Subject = c.Subject,
                    Quantity = c.Quantity,
                    Amount = c.Amount,
                    Status = c.Status
                }).ToListAsync();

            if (contracts.Count == 0)
            {
                return NotFound(new { message = "Договорів для цього проєкту не знайдено." });
            }

            return Ok(contracts);
        }

        // ОНОВИТИ СТАТУС ДОГОВОРУ (наприклад, "виконано")
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateContractStatus(int id, ContractUpdateDTO dto)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound(new { message = "Договір з таким ID не знайдено." });
            }

            contract.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Статус договору успішно оновлено!" });
        }
    }
}