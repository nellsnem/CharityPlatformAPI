using System.ComponentModel.DataAnnotations.Schema;

namespace CharityPlatformAPI.DTOs
{
    public class VolunteerResponseDTO
    {
        public int UserId { get; set; }
        public int VolunteerId { get; set; }
        public decimal? Rating { get; set; }
        [Column("status")]
        public string Status { get; set; } = null!;
    }
}
