using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class VolunteerShift
{
    public int ShiftId { get; set; }

    public int ProjectId { get; set; }

    public int EmployeeId { get; set; }

    public int VolunteerId { get; set; }

    public int HoursWorked { get; set; }

    public DateOnly ShiftDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual Volunteer Volunteer { get; set; } = null!;
}
