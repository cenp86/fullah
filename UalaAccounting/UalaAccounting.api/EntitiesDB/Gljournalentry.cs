using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Gljournalentry
{
    public string Encodedkey { get; set; } = null!;

    public string? Accountkey { get; set; }

    public decimal Amount { get; set; }

    public long Entryid { get; set; }

    public DateTime? Creationdate { get; set; }

    public string GlaccountEncodedkeyOid { get; set; } = null!;

    public string? Notes { get; set; }

    public string? Producttype { get; set; }

    public DateTime? Entrydate { get; set; }

    public string Transactionid { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Userkey { get; set; }

    public string? Assignedbranchkey { get; set; }

    public string? Reversalentrykey { get; set; }

    public string? Productkey { get; set; }

    public virtual Branch? AssignedbranchkeyNavigation { get; set; }

    public virtual Glaccount GlaccountEncodedkeyO { get; set; } = null!;

    public virtual ICollection<Gljournalentry> InverseReversalentrykeyNavigation { get; set; } = new List<Gljournalentry>();

    public virtual Gljournalentry? ReversalentrykeyNavigation { get; set; }
}
