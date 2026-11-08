using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class ContractCreateDTO
    {
        [Required]
        public int SupplierId { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Дата підписання є обов'язковою.")]

        public DateOnly SigningDate { get; set; }
        [Required(ErrorMessage = "Предмет договору не може бути порожнім.")]
        [StringLength(255, ErrorMessage = "Назва предмету договору занадто довга.")]
        public string Subject { get; set; } = null!;
        [Required]
        [Range(1, 1000000, ErrorMessage = "Кількість повинна бути більшою за нуль.")]
        public int Quantity { get; set; }
        [Required]
        [Range(0.01, 100000000, ErrorMessage = "Сума договору має бути більшою за нуль.")]
        public decimal Amount { get; set; }
    }
}
