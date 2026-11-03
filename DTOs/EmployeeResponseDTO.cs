namespace CharityPlatformAPI.DTOs
{
    public class EmployeeResponseDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public string Login { get; set; } = null!;
    }
}
