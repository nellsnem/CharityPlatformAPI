namespace CharityPlatformAPI.DTOs
{
    public class ProjectCreateDTO
    {
        public int HelpRequestId { get; set; }
        public int EmployeeId { get; set; }
        public string Title { get; set; } = null!; 
        public string Description { get; set; } = null!; 
        public decimal ApprovedBudget { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
