using System;
using System.Collections.Generic;

namespace CharityPlatformAPI.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public int SupplierId { get; set; }

    public int ProjectId { get; set; }

    public DateOnly SigningDate { get; set; }

    public string Subject { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<FinanceTransaction> FinanceTransactions { get; set; } = new List<FinanceTransaction>();

    public virtual Project Project { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
