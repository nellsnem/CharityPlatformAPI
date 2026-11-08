using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.Controllers
{
    namespace CharityPlatformAPI.DTOs
    { 
        public class SupplierCreateDTO
        {
            [Required(ErrorMessage = "Назва компанії є обов'язковою.")]
            [StringLength(200)]
            public string CompanyName { get; set; } = null!;

            [Required(ErrorMessage = "Код ЄДРПОУ є обов'язковим.")]
            [RegularExpression(@"^\d{8}$", ErrorMessage = "Код ЄДРПОУ має містити рівно 8 цифр.")]
            public string EdrpouCode { get; set; } = null!;

            public string? Contacts { get; set; }
        }
    }
}
