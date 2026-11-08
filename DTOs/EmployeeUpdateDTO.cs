using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class EmployeeUpdateDTO
    {
        [Required(ErrorMessage = "Посада є обов'язковою.")]
        [StringLength(100)]
        public string Position { get; set; } = null!;

        [Required(ErrorMessage = "Номер телефону є обов'язковим.")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Номер телефону має бути у форматі +380XXXXXXXXX.")]
        public string ContactPhone { get; set; } = null!;
    }
}
