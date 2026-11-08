using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class VolunteerCreateDTO
    {
        [Required(ErrorMessage = "ID користувача є обов'язковим.")]
        public int UserId { get; set; }
    }
}
