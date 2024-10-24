using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Accountinginterestaccrualbreakdown
{
    public ulong Id { get; set; }

    public string Transactionid { get; set; } = null!;

    public long Entryid { get; set; }

    public string Entrytype { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime Creationdate { get; set; }

    public DateTime Bookingdate { get; set; }

    public string Glaccountencodedkey { get; set; } = null!;

    public string Glaccounttype { get; set; } = null!;

    public string Accountencodedkey { get; set; } = null!;

    public string Accountid { get; set; } = null!;

    public string? Branchencodedkey { get; set; }

    public string Productencodedkey { get; set; } = null!;

    public string Producttype { get; set; } = null!;

    public bool Sent { get; set; }

    public bool Processed { get; set; }

    public string Accrualtype { get; set; } = null!;

    public decimal? Foreignamount { get; set; }

    public string? Accountingratekey { get; set; }

    public string? Currencycode { get; set; }
}
