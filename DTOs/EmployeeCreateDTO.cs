using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class EmployeeCreateDTO
    {
        [Required(ErrorMessage = "ПІБ є обов'язковим.")]
        [StringLength(150, ErrorMessage = "ПІБ не може перевищувати 150 символів.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Посада є обов'язковою.")]
        [StringLength(100)]
        public string Position { get; set; } = null!;

        [Required(ErrorMessage = "Номер телефону є обов'язковим.")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Номер телефону має бути у форматі +380XXXXXXXXX.")]
        public string ContactPhone { get; set; } = null!;

        [Required(ErrorMessage = "Логін є обов'язковим.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логін має містити від 3 до 50 символів.")]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "Пароль є обов'язковим.")]
        [MinLength(6, ErrorMessage = "Пароль має містити мінімум 6 символів.")]
        public string Password { get; set; } = null!;
    }
}
