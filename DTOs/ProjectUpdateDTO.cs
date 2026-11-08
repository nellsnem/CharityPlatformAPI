using System.ComponentModel.DataAnnotations;

namespace CharityPlatformAPI.DTOs
{
    public class ProjectUpdateDTO
    {
        [Required(ErrorMessage = "Для закриття проєкту необхідно вказати кінцеву дату.")]
        public DateOnly? EndDate { get; set; }
    }
}
