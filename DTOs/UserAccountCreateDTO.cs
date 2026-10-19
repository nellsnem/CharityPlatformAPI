using System.ComponentModel.DataAnnotations;
namespace CharityPlatformAPI.DTOs
{
    public class UserAccountCreateDTO
    {
        [Required(ErrorMessage = "Ім'я є обов'язковим!")]
        [MinLength(3, ErrorMessage = "Ім'я занадто коротке!")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Введіть номер телефону!")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "Номер телефону має містити від 10 до 13 символів.")]
        [RegularExpression(@"^\+?[0-9]+$", ErrorMessage = "Номер телефону може містити лише цифри та знак плюса на початку.")]
        public string Phone { get; set; } = null!;
        [Required(ErrorMessage = "Пароль обов'язковий!")]
        [MinLength(4, ErrorMessage = "Пароль має містити мінімум 4 символи!")]
        public string Password { get; set; } = null!;
    }
}