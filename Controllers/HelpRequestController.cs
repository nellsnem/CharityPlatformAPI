using CharityPlatformAPI.DTOs;
using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpRequestController : ControllerBase
    {
        private readonly CharityContext _context;
        public HelpRequestController(CharityContext context)
        {
            _context = context;
        }
        //Створює новий запис
        [HttpPost]
        public async Task<ActionResult> CreateHelpRequest(HelpRequestCreateDTO dto)
        {

            bool beneficiaryExists = await _context.Beneficiaries.AnyAsync(u => u.BeneficiaryId == dto.BeneficiaryId);

            if (!beneficiaryExists)
            {
                return NotFound(new { message = "Такого бенефіціара немає." });

            }

            var newRequest = new HelpRequest
            {
                BeneficiaryId = dto.BeneficiaryId,
                NeedDescription = dto.NeedDescription,
                DeliveryAddress = dto.DeliveryAddress,
                DocumentPath = dto.DocumentPath,
                Status = "нове"

            };
            _context.HelpRequests.Add(newRequest);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Запит успішно створено!" });

        }
        //ПОКАЗУЄ СПИСОК ЗАПИТІВ
        [HttpGet]
        public async Task<ActionResult> GetAllHelpRequests()
        {
            var list = await _context.HelpRequests
                .Select(x => new HelpRequestResponseDTO
                {
                    RequestId = x.RequestId,
                    BeneficiaryId = x.BeneficiaryId,
                    EmployeeId = x.EmployeeId,
                    RegistrationDate = x.RegistrationDate,
                    NeedDescription = x.NeedDescription,
                    DocumentPath = x.DocumentPath,
                    DeliveryAddress = x.DeliveryAddress,
                    Status = x.Status

                }).ToListAsync();
            return Ok(list);
        }
        // пошук запиту за ід
        [HttpGet("{id}")]
        public async Task<ActionResult> GetHelpRequestById(int id)
        {
            var request  = await _context.HelpRequests.FindAsync(id);

            if(request == null)
            {
                return NotFound("Запит з таким ID не знайдено.");
            }

            var responseDto = new HelpRequestResponseDTO
            {
                RequestId = request.RequestId,
                BeneficiaryId = request.BeneficiaryId,
                EmployeeId = request.EmployeeId,
                RegistrationDate = request.RegistrationDate,
                NeedDescription = request.NeedDescription,
                DocumentPath = request.DocumentPath,
                DeliveryAddress = request.DeliveryAddress,
                Status = request.Status
            };
            return Ok(responseDto);
        }
        // пошук запиту за ід бенефіціара
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRequestsByBeneficiaryId(int id, BeneficiaryResponseDTO dto)
        {
            var request = await _context.Beneficiaries.FindAsync(id);

            if (request == null)
            {
                return NotFound("Запит з таким ID не знайдено.");
            }

            var responseDto = new BeneficiaryResponseDTO
            {
                RequestId = request.RequestId,
                BeneficiaryId = request.BeneficiaryId,
                EmployeeId = request.EmployeeId,
                RegistrationDate = request.RegistrationDate,
                NeedDescription = request.NeedDescription,
                DocumentPath = request.DocumentPath,
                DeliveryAddress = request.DeliveryAddress,
                Status = request.Status
            };
            return Ok(responseDto);
        }

    }
}

/*

GetRequestsByBeneficiaryId (GET beneficiary/{beneficiaryId}): Шукає всі запити, де id_бенефіціара збігається з переданим. Потрібно для особистого кабінету користувача, щоб він бачив історію своїх заявок.   

UpdateHelpRequestStatus (PUT {id}/status): Працівник оновлює статус запиту і записує свій EmployeeId у базу, щоб було видно, хто саме обробив заявку.   
 */