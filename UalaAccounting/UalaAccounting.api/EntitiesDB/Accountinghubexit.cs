using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Accountinghubexit
{
    public string Loanid { get; set; } = null!;

    public string Productencodedkey { get; set; } = null!;

    public string? Branchname { get; set; }

    public string? Actualstage { get; set; }

    public string? Laststagechange { get; set; }

    public string Glcode { get; set; } = null!;

    public string Glname { get; set; } = null!;

    public string Transactionid { get; set; } = null!;

    public long Entryid { get; set; }

    public string? Transactionidrev { get; set; }

    public long? Entryidrev { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Creationdate { get; set; }

    public DateTime? BookingDate { get; set; }

    public string Type { get; set; } = null!;

    public string? Loantransactiontype { get; set; }

    public string? Mambuglcode { get; set; }
}
