namespace CharityPlatformAPI.DTOs
{
    public class SupplierUpdateDTO
    {
        public string CompanyName { get; set; } = null!;
        public string EdrpouCode { get; set; } = null!;
        public string? Contacts { get; set; }
    }
}
