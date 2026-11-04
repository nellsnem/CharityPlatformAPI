using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly CharityContext _context;

        public FinanceController(CharityContext context)
        {
            _context = context;
        }

        // СТВОРИТИ ТРАНЗАКЦІЮ 
        [HttpPost]
        public async Task<ActionResult> CreateTransaction(FinanceCreateDTO dto)
        {
            var transaction = new FinanceTransaction
            {
                ProjectId = dto.ProjectId,
                ContractId = dto.ContractId,
                EmployeeId = dto.EmployeeId,
                Amount = dto.Amount,
                Type = dto.Type,
                Purpose = dto.Purpose,
                TransactionDate = dto.TransactionDate
            };

            _context.FinanceTransactions.Add(transaction);
            await _context.SaveChangesAsync(); 

            
            if (dto.ProjectId.HasValue && dto.Type.ToLower() == "надходження") 
            {
                var project = await _context.Projects.FindAsync(dto.ProjectId.Value);
                if (project != null && project.Status != "завершено")
                {
                    var totalIncome = await _context.FinanceTransactions
                        .Where(t => t.ProjectId == dto.ProjectId.Value && t.Type.ToLower() == "надходження")
                        .SumAsync(t => t.Amount);

                    if (totalIncome >= project.ApprovedBudget)
                    {
                        project.Status = "завершено";
                        project.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);

                        await _context.SaveChangesAsync(); 

                        return Ok(new { message = $"Транзакцію додано! Бюджет зібрано ({totalIncome}/{project.ApprovedBudget}). Проєкт автоматично закрито!" });
                    }
                }
            }

            return Ok(new { message = "Фінансову транзакцію успішно створено!" });
        }

        // ЗНАЙТИ ВСІ ТРАНЗАКЦІЇ
        [HttpGet]
        public async Task<ActionResult> GetAllTransactions()
        {
            var list = await _context.FinanceTransactions
                .Select(x => new FinanceResponseDTO
                {
                    TransactionId = x.TransactionId,
                    ProjectId = x.ProjectId,
                    ContractId = x.ContractId,
                    EmployeeId = x.EmployeeId,
                    Amount = x.Amount,
                    Type = x.Type,
                    Purpose = x.Purpose,
                    TransactionDate = x.TransactionDate
                }).ToListAsync();

            return Ok(list);
        }

        // ЗНАЙТИ ТРАНЗАКЦІЮ ЗА АЙДІ
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTransactionById(int id)
        {
            var t = await _context.FinanceTransactions.FindAsync(id);

            if (t == null)
            {
                return NotFound(new { message = "Транзакцію з таким ID не знайдено." });
            }

            var responseDto = new FinanceResponseDTO
            {
                TransactionId = t.TransactionId,
                ProjectId = t.ProjectId,
                ContractId = t.ContractId,
                EmployeeId = t.EmployeeId,
                Amount = t.Amount,
                Type = t.Type,
                Purpose = t.Purpose,
                TransactionDate = t.TransactionDate
            };

            return Ok(responseDto);
        }

        // ЗНАЙТИ ТРАНЗАКЦІЇ ЗА АЙДІ ПРОЄКТУ (Звітність)
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult> GetTransactionsByProjectId(int projectId)
        {
            var transactions = await _context.FinanceTransactions
                .Where(t => t.ProjectId == projectId)
                .Select(x => new FinanceResponseDTO
                {
                    TransactionId = x.TransactionId,
                    ProjectId = x.ProjectId,
                    ContractId = x.ContractId,
                    EmployeeId = x.EmployeeId,
                    Amount = x.Amount,
                    Type = x.Type,
                    Purpose = x.Purpose,
                    TransactionDate = x.TransactionDate
                }).ToListAsync();

            if (transactions.Count == 0)
            {
                return NotFound(new { message = "Транзакцій для цього проєкту не знайдено." });
            }

            return Ok(transactions);
        }
    }
}