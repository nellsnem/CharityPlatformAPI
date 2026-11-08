using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class ParticipationUpdateDTO
    {
        [Required]
        [RegularExpression("^(зареєстровано|був присутній|не з'явився)$", ErrorMessage = "Недопустимий статус присутності.")]
        public string StatusPresence { get; set; } = null!;

        [Range(0, 24, ErrorMessage = "Кількість годин має бути від 0 до 24.")]
        public decimal? HoursWorked { get; set; }
    }
}
