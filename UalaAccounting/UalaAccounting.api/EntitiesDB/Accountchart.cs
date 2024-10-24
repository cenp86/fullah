using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Accountchart
{
    public string Encodedkey { get; set; } = null!;

    public string? Accountchartid { get; set; }

    public string? Glcode { get; set; }

    public string? Glname { get; set; }

    public string? Type { get; set; }

    public string? Owner { get; set; }

    public bool? Enable { get; set; }
}
