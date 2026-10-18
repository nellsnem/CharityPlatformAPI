using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class Volunteer
{
    public int VolunteerId { get; set; }

    public int UserId { get; set; }

    public decimal? Rating { get; set; }

    public virtual UserAccount User { get; set; } = null!;

    public virtual ICollection<VolunteerShift> VolunteerShifts { get; set; } = new List<VolunteerShift>();
}
