using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string EdrpouCode { get; set; } = null!;

    public string? Contacts { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
