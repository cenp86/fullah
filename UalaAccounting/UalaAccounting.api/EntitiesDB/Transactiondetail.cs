using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Transactiondetail
{
    public string Encodedkey { get; set; } = null!;

    public string? Transactionchannelkey { get; set; }

    public ulong Internaltransfer { get; set; }

    public string? Targetsavingsaccountkey { get; set; }

    public virtual ICollection<Loantransaction> Loantransactions { get; set; } = new List<Loantransaction>();

    public virtual Transactionchannel? TransactionchannelkeyNavigation { get; set; }
}
