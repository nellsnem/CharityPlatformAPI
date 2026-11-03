namespace CharityPlatformAPI.DTOs
{
    public class EmployeeCreateDTO
    {
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
