using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class UserAccountCreateDTO
    {
        [Required(ErrorMessage = "Поле ПІБ є обов'язковим.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "ПІБ має містити від 3 до 150 символів.")]
        [RegularExpression(@"^[А-Яа-яІіЇїЄєҐґA-Za-z\s\-\']+$", ErrorMessage = "Ім'я може містити лише літери.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Номер телефону є обов'язковим.")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Номер телефону має бути у форматі +380XXXXXXXXX.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим.")]
        [MinLength(4, ErrorMessage = "Пароль має містити мінімум 4 символи.")]
        public string Password { get; set; }
    }
}