namespace CharityPlatformAPI.DTOs
{
    public class ProjectUpdateDTO
    {
        public string Status { get; set; } = null!;
        public DateOnly? EndDate { get; set; }
    }
}
