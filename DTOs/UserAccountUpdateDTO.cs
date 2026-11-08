using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class UserAccountUpdateDTO
    {
        [Required(ErrorMessage = "Ім'я є обов'язковим!")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Ім'я має містити від 3 до 150 символів!")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Введіть номер телефону!")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Номер телефону має бути у форматі +380XXXXXXXXX.")]
        public string Phone { get; set; } = null!;
    }
}
