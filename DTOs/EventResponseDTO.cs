namespace CharityPlatformAPI.DTOs
{
    public class EventResponseDTO
    {
        public int EventId { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = null!;
        public string? Description { get; set; }
    }
}
