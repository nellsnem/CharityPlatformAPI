namespace CharityPlatformAPI.DTOs
{
    public class ProjectResponseDTO
    {
        public int ProjectId { get; set; }
        public int HelpRequestId { get; set; }
        public int EmployeeId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal ApprovedBudget { get; set; }
        public string Status { get; set; } = null!;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
