using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class ProjectCreateDTO
    {
        [Required]
        public int HelpRequestId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Назва проєкту є обов'язковою.")]
        [StringLength(200, ErrorMessage = "Назва проєкту не може перевищувати 200 символів.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Опис проєкту є обов'язковим.")]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0, 100000000, ErrorMessage = "Бюджет не може бути від'ємним.")]
        public decimal ApprovedBudget { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
