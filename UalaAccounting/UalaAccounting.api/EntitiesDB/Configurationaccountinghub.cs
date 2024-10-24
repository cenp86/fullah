using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Configurationaccountinghub
{
    public string Encodedkey { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Valueconfiguration { get; set; }

    public bool? Enable { get; set; }
}
