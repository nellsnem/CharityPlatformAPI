namespace CharityPlatformAPI.Controllers
{
    public class SupplierCreateDTO
    {
        public string CompanyName { get; set; } = null!;
        public string EdrpouCode { get; set; } = null!;
        public string? Contacts { get; set; }
    }
}
