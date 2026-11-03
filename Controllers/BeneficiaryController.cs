using CharityPlatformAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using CharityPlatformAPI.DTOs;
using Microsoft.AspNetCore.Identity;


namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {

        private readonly CharityContext _context;
        public BeneficiaryController(CharityContext context)
        {
            _context = context;
        }
        //створити бенефеціара
        [HttpPost]
        public async Task<ActionResult> CreateBeneficiary(BeneficiaryCreateDTO dto)
        {
            bool userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.UserId);

            if (!userExists)
            {
                return NotFound("Користувача з таким ID не знайдено в системі.");

            }

            var existingBeneficiary = await _context.Beneficiaries.FirstOrDefaultAsync(v => v.UserId == dto.UserId);
            if (existingBeneficiary != null)
            {
                if (existingBeneficiary.Status == "заблокований")
                {
                    return BadRequest("Вам назавжди заборонено реєструвати запити через порушення правил.");
                }


                return BadRequest("Цей користувач вже є бенефеціаром.");

            }
            bool passportExists = await _context.Beneficiaries.AnyAsync(b => b.PassportNumber == dto.PassportNumber);
            if (passportExists)
            {
                return BadRequest("Особа з таким номером паспорта вже зареєстрована в системі.");
            }

            var newBeneficiary = new Beneficiary
            {
                UserId = dto.UserId,
                PassportNumber = dto.PassportNumber,
                Status = "активний"
            };
            _context.Beneficiaries.Add(newBeneficiary);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Бенефеціара успішно створено!" });
        }
        //ЗНАЙТИ ВСІХ БЕНЕФІЦІАРІВ
        [HttpGet]
        public async Task<ActionResult> GetAllBeneficiaries()
        {
            var list = await _context.Beneficiaries
                .Select(b => new BeneficiaryResponseDTO
                {
                    UserId = b.UserId,
                    PassportNumber = b.PassportNumber,
                }).ToListAsync();

            return Ok(list);
        }
        //ЗНАЙТИ БЕНЕФІЦІАРА ЗА ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetBeneficiaryById(int id)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(id);
            if (beneficiary == null)
            {
                return NotFound("Бенефеціара з таким ID не знайдено.");

            }
            var responseDto = new BeneficiaryResponseDTO
            {
                UserId = beneficiary.UserId,
                PassportNumber = beneficiary.PassportNumber,
                Status = beneficiary.Status

            };
            return Ok(responseDto);
        }
        //ОНОВИТИ ПАСПОРТНІ ДАНІ
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBeneficiary(int id, BeneficiaryUpdateDTO dto)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(id);

            if (beneficiary == null)
            {
                return NotFound("Бенефеціара з таким ID не знайдено.");
            }

            if (beneficiary.PassportNumber != dto.PassportNumber)
            {
                bool passportExists = await _context.Beneficiaries.AnyAsync(b => b.PassportNumber == dto.PassportNumber);
                if (passportExists)
                {
                    return BadRequest("Цей паспорт вже зареєстрований в системі.");
                }
            }

            beneficiary.PassportNumber = dto.PassportNumber;

            await _context.SaveChangesAsync();
            return Ok("Дані користувача успішно оновлено!");

        }
        //ЗАБЛОКУВАТИ БЕНЕФІЦІАРА
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBeneficiary(int id)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(id);
            if(beneficiary == null)
            {
                return NotFound("Бенефеціара з таким ID не знайдено.");
            }

            beneficiary.Status = "заблокований";
            await _context.SaveChangesAsync();
            return Ok("Бенефеціара успішно заблоковано!");
        }
        //ПЕРЕВІРИТИ СТАТУС
        [HttpGet("check/{userId}")]
        public async Task<ActionResult> CheckBeneficiaryStatus(int userId)
        {
            var beneficiary = await _context.Beneficiaries.FirstOrDefaultAsync(v => v.UserId == userId);

            if(beneficiary == null)
            {
                return Ok(new { isBeneficiary = false, message = "Користувач не є бенефеціаром." });

            }

            var responseDto = new BeneficiaryResponseDTO
            {
                BeneficiaryId = beneficiary.BeneficiaryId,
                UserId = beneficiary.BeneficiaryId,
                PassportNumber = beneficiary.PassportNumber,
                Status = beneficiary.Status,

            };

            return Ok(new { isBeneficiary = true, beneficiaryData = responseDto });

        }

    }
}
