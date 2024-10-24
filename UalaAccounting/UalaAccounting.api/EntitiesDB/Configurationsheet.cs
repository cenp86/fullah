using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Configurationsheet
{
    public string Encodedkey { get; set; } = null!;

    public string? Name { get; set; }

    public int? Startposition { get; set; }

    public int? Startpositioncolumnname { get; set; }

    public int? Numbercolumns { get; set; }

    public bool? Requiredid { get; set; }

    public int? Startpositionid { get; set; }

    public bool? Reclassification { get; set; }

    public bool? Enable { get; set; }
}
