using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Transactionchannel
{
    public string Encodedkey { get; set; } = null!;

    public string? Id { get; set; }

    public string? Name { get; set; }

    public int Index { get; set; }

    public string? Createdbyuserkey { get; set; }

    public DateTime? Creationdate { get; set; }

    public ulong Activated { get; set; }

    public string Savingsconstraintsusage { get; set; } = null!;

    public string Loanconstraintsusage { get; set; } = null!;

    public string? Usagerightskey { get; set; }

    public string? LoanCustomFilterConstraintKey { get; set; }

    public string? SavingsCustomFilterConstraintKey { get; set; }

    public virtual ICollection<Transactiondetail> Transactiondetails { get; set; } = new List<Transactiondetail>();
}
