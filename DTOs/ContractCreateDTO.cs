namespace CharityPlatformAPI.DTOs
{
    public class ContractCreateDTO
    {
        public int SupplierId { get; set; }
        public int ProjectId { get; set; }
        public DateOnly SigningDate { get; set; }
        public string Subject { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
