using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Productaccountinghub
{
    public string Productencodedkey { get; set; } = null!;

    public string? Productid { get; set; }

    public string? Accountchart { get; set; }

    public bool? Enable { get; set; }
}
