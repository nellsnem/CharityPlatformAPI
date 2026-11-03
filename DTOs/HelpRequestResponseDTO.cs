namespace CharityPlatformAPI.DTOs
{
    public class HelpRequestResponseDTO
    {
        public int RequestId { get; set; }
        public int BeneficiaryId { get; set; }
        public int? EmployeeId { get; set; }
        public DateOnly RegistrationDate { get; set; }
        public string NeedDescription { get; set; } = null!;
        public string? DocumentPath { get; set; }
        public string? DeliveryAddress { get; set; }
        public string Status { get; set; } = null!;
    }
}
