using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public string? ContactPhone { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<FinanceTransaction> FinanceTransactions { get; set; } = new List<FinanceTransaction>();

    public virtual ICollection<HelpRequest> HelpRequests { get; set; } = new List<HelpRequest>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<VolunteerShift> VolunteerShifts { get; set; } = new List<VolunteerShift>();
}
