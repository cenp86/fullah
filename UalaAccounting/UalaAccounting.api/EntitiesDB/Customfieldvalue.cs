using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Customfieldvalue
{
    public string Encodedkey { get; set; } = null!;

    public string Customfieldkey { get; set; } = null!;

    public int? Indexinlist { get; set; }

    public string Parentkey { get; set; } = null!;

    public string? Value { get; set; }

    public string? Linkedentitykeyvalue { get; set; }

    public decimal? Amount { get; set; }

    public int Customfieldsetgroupindex { get; set; }
}
