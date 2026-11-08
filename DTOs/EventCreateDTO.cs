using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class EventCreateDTO
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Назва заходу є обов'язковою.")]
        [StringLength(200, ErrorMessage = "Назва не може перевищувати 200 символів.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Дата заходу є обов'язковою.")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Місце проведення є обов'язковим.")]
        [StringLength(255)]
        public string Location { get; set; } = null!;

        public string? Description { get; set; }
    }
}
