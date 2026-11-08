namespace CharityPlatformAPI.DTOs
{
    public class SupplierResponseDTO
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string EdrpouCode { get; set; } = null!;
        public string? Contacts { get; set; }
    }
}
