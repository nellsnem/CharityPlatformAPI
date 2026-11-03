using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CharityPlatformAPI.Models
{
    [Table("event")]
    public class Event
    {
        [Key]
        [Column("event_id")]
        public int EventId { get; set; }

        [Column("project_id")]
        public int ProjectId { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Column("title")]
        public string Title { get; set; } = null!;

        [Column("event_date")]
        public DateTime EventDate { get; set; }

        [Column("location")]
        public string Location { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }
    }
}