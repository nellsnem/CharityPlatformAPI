namespace CharityPlatformAPI.DTOs
{
    public class ContractResponseDTO
    {
        public int ContractId { get; set; }
        public int SupplierId { get; set; }
        public int ProjectId { get; set; }
        public DateOnly SigningDate { get; set; }
        public string Subject { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
    }
}
