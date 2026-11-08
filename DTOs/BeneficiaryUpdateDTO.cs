using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class BeneficiaryUpdateDTO
    {
        [Required(ErrorMessage = "Номер паспорта є обов'язковим.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Номер паспорта має містити від 6 до 20 символів.")]
        public string PassportNumber { get; set; }
    }
}
