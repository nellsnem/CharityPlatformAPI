using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class UserAccount
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual Beneficiary? Beneficiary { get; set; }

    public virtual Volunteer? Volunteer { get; set; }
}
