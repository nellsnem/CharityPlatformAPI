using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class FinanceTransaction
{
    public int TransactionId { get; set; }

    public int? ProjectId { get; set; }

    public int? ContractId { get; set; }

    public int? EmployeeId { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = null!;

    public string? Purpose { get; set; }

    public DateOnly TransactionDate { get; set; }

    public virtual Contract? Contract { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Project? Project { get; set; }
}
