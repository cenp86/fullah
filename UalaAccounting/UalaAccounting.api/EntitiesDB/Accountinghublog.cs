using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Accountinghublog
{
    public int Logid { get; set; }

    public string Logline { get; set; } = null!;

    public DateTime Creationdate { get; set; }
}
