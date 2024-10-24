using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Configurationsheetcolumn
{
    public string Encodedkey { get; set; } = null!;

    public string Configurationsheetencodedkey { get; set; } = null!;

    public string? Name { get; set; }

    public int? Columnindex { get; set; }

    public bool? Required { get; set; }

    public bool? Enable { get; set; }
}
