namespace CharityPlatformAPI.DTOs
{
    public class ParticipationResponseDTO
    {
        public int ParticipationId { get; set; }
        public int EventId { get; set; }
        public int VolunteerId { get; set; }
        public int? EmployeeId { get; set; }
        public string StatusPresence { get; set; } = null!;
        public decimal? HoursWorked { get; set; }
    }
}
