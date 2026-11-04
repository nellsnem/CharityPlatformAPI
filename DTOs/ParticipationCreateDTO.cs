namespace CharityPlatformAPI.DTOs
{
    public class ParticipationCreateDTO
    {
        public int EventId { get; set; }
        public int VolunteerId { get; set; }
        public int? EmployeeId { get; set; }
    }
}
