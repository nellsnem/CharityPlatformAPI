using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class SupplierCreateDTO
    {
        [Required(ErrorMessage = "Назва компанії є обов'язковою.")]
        [MinLength(3, ErrorMessage = "Назва компанії має містити щонайменше 3 символи.")]
        public string CompanyName { get; set; } = null!;

        [Required(ErrorMessage = "Код ЄДРПОУ є обов'язковим.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Код ЄДРПОУ повинен містити рівно 8 цифр.")]
        public string EdrpouCode { get; set; } = null!;

        public string? Contacts { get; set; }
    }
}