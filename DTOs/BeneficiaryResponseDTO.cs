namespace CharityPlatformAPI.DTOs
{
    public class BeneficiaryResponseDTO
    {
        public int BeneficiaryId { get; set; }  
        public int UserId { get; set; }
        public string PassportNumber { get; set; }
        public string Status { get; set; }

    }
}
