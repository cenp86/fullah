using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Accountinghubentry
{
    public string Loanid { get; set; } = null!;

    public string Productencodedkey { get; set; } = null!;

    public string? Branchname { get; set; }

    public string? Actualstage { get; set; }

    public string? Laststagechange { get; set; }

    public string? Glcode { get; set; }

    public string Glname { get; set; } = null!;

    public string Transactionid { get; set; } = null!;

    public long Entryid { get; set; }

    public decimal Amount { get; set; }

    public DateTime Creationdate { get; set; }

    public DateTime? BookingDate { get; set; }

    public string Type { get; set; } = null!;

    public string? Loantransactiontype { get; set; }

    public decimal Principaldue { get; set; }

    public decimal Principalbalance { get; set; }

    public decimal Interestdue { get; set; }

    public decimal Interestbalance { get; set; }

    public decimal Feesdue { get; set; }

    public decimal Feesbalance { get; set; }

    public decimal Penaltydue { get; set; }

    public decimal Penaltybalance { get; set; }

    public string? Reversaltransactionid { get; set; }

    public bool IsPayoff { get; set; }

    public bool IsPrepayment { get; set; }

    public bool IsOverdue { get; set; }

    public string? Transactionchannel { get; set; }
}
