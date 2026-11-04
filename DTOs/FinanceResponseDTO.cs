namespace CharityPlatformAPI.DTOs
{
    public class FinanceResponseDTO
    {
        public int TransactionId { get; set; }
        public int? ProjectId { get; set; }
        public int? ContractId { get; set; }
        public int? EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = null!;
        public string? Purpose { get; set; }
        public DateOnly TransactionDate { get; set; }
    }
}
