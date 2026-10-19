namespace CharityPlatformAPI.DTOs
{
    public class UserAccountResponseDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null;

        public string Phone { get; set; } = null;
    }
}
