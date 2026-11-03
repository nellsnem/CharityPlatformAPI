namespace CharityPlatformAPI.DTOs
{
    public class HelpRequestCreateDTO
    {
        public int BeneficiaryId { get; set; }
        public string NeedDescription { get; set; } = null!;
        public string? DocumentPath { get; set; }
        public string? DeliveryAddress { get; set; }
    }
}