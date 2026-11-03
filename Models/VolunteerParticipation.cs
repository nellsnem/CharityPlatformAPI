using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CharityPlatformAPI.Models
{
    [Table("volunteer_participation")]
    public class VolunteerParticipation
    {
        [Key]
        [Column("participation_id")]
        public int ParticipationId { get; set; }

        [Column("event_id")]
        public int EventId { get; set; }

        [Column("volunteer_id")]
        public int VolunteerId { get; set; }

        [Column("employee_id")]
        public int? EmployeeId { get; set; } 

        [Column("status_presence")]
        public string StatusPresence { get; set; } = null!;

        [Column("hours_worked")]
        public decimal? HoursWorked { get; set; } 
    }
}