using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class VolunteerUpdateDTO
    {
        [Range(0, 5, ErrorMessage = "Рейтинг волонтера має бути в межах від 0 до 5.")]
        public decimal? Rating { get; set; }
    }
}
