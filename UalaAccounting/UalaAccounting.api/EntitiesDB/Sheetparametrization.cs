using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Sheetparametrization
{
    public string Encodedkey { get; set; } = null!;

    /// <summary>
    /// ACCOUNTCHART
    /// </summary>
    public string? Accountchart { get; set; }

    public DateTime? Creationdate { get; set; }

    public string? Currentstage { get; set; }

    public string? Currentglcode { get; set; }

    public string? Currentloantrxtype { get; set; }

    public string? Currentjetype { get; set; }

    public string? Outputglcode { get; set; }

    public string? Outputglname { get; set; }

    public string? Outputjetype { get; set; }

    public string? Exclusionglcodes { get; set; }

    public bool Adjust { get; set; }

    public string? Observaciones { get; set; }

    public bool Enable { get; set; }

    public bool Taxentry { get; set; }

    public bool Principal { get; set; }

    public bool Overdueppal { get; set; }

    public string? Amount { get; set; }
}
