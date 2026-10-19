using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CharityPlatformAPI.Models;

public partial class Beneficiary
{
    public int BeneficiaryId { get; set; }

    public int UserId { get; set; }

    public string PassportNumber { get; set; } = null!;

    public virtual ICollection<HelpRequest> HelpRequests { get; set; } = new List<HelpRequest>();

    public virtual UserAccount User { get; set; } = null!;
    [Column("status")]
    public string Status { get; set; } = null!;
}
