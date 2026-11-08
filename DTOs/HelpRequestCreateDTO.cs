using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class HelpRequestCreateDTO
    {
        [Required]
        public int BeneficiaryId { get; set; }

        [Required(ErrorMessage = "Опис потреби є обов'язковим.")]
        public string NeedDescription { get; set; } = null!;

        public string? DocumentPath { get; set; }
        public string? DeliveryAddress { get; set; }
    }
}