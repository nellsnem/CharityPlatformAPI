using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class FinanceCreateDTO
    {
        public int? ProjectId { get; set; }
        public int? ContractId { get; set; }
        public int? EmployeeId { get; set; }

        [Required]
        [Range(0.01, 100000000, ErrorMessage = "Сума транзакції повинна бути більшою за нуль.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Тип транзакції є обов'язковим.")]
        [RegularExpression("^(надходження|витрата)$", ErrorMessage = "Тип може бути лише 'надходження' або 'витрата'.")]
        public string Type { get; set; } = null!;

        public string? Purpose { get; set; }

        [Required]
        public DateOnly TransactionDate { get; set; }
    }
}
