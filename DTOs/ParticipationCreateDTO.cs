using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class ParticipationCreateDTO
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int VolunteerId { get; set; }

        public int? EmployeeId { get; set; }
    }
}
