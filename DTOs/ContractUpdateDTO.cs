using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class ContractUpdateDTO
    {
        [Required(ErrorMessage = "Статус є обов'язковим.")]
        [RegularExpression("^(чинний|виконано)$", ErrorMessage = "Статус може бути лише 'чинний' або 'виконано'.")]
        public string Status { get; set; } = null!;
    }
}
