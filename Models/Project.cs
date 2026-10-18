using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public int RequestId { get; set; }

    public int EmployeeId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal ApprovedBudget { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<FinanceTransaction> FinanceTransactions { get; set; } = new List<FinanceTransaction>();

    public virtual HelpRequest Request { get; set; } = null!;

    public virtual ICollection<VolunteerShift> VolunteerShifts { get; set; } = new List<VolunteerShift>();
}
