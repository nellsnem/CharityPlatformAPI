using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class HelpRequestStatusUpdateDTO
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [RegularExpression("^(нове|схвалено|відхилено)$", ErrorMessage = "Недопустимий статус запиту.")]
        public string Status { get; set; } = null!;
    }
}
