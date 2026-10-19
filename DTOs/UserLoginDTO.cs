using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Введіть номер телефону!")]
        public string Phone { get; set; } = null!;
        [Required(ErrorMessage = "Введіть пароль!")]
        public string Password { get; set; } = null!;
    }
}
