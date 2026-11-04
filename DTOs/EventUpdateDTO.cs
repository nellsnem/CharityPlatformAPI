namespace CharityPlatformAPI.DTOs
{
    public class EventUpdateDTO
    {
        public string Title { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = null!;
        public string? Description { get; set; }
    }
}
