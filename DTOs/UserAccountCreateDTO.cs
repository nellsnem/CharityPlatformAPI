using System.ComponentModel.DataAnnotations;
namespace CharityPlatformAPI.DTOs
{
    public class UserAccountCreateDTO
    {
        [Required(ErrorMessage = "Ім'я є обов'язковим!")]
        [MinLength(3, ErrorMessage = "Ім'я занадто коротке!")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Номер телефону обов'язковий!")]
        [Phone(ErrorMessage = "Невірний формат телефону!")]
        public string Phone { get; set; } = null!;
        [Required(ErrorMessage = "Пароль обов'язковий!")]
        [MinLength(4, ErrorMessage = "Пароль має містити мінімум 4 символи!")]
        public string Password { get; set; } = null!;
    }
}