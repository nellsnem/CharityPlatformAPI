using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class HelpRequest
{
    public int RequestId { get; set; }

    public int BeneficiaryId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public string NeedDescription { get; set; } = null!;

    public string? DocumentPath { get; set; }

    public string? DeliveryAddress { get; set; }

    public string Status { get; set; } = null!;

    public virtual Beneficiary Beneficiary { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual Project? Project { get; set; }
}
