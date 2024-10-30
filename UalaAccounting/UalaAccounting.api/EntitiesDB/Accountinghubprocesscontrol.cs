using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Accountinghubprocesscontrol
{
    public string Processuuid { get; set; } = null!;

    public DateTime? Startdate { get; set; }

    public DateTime? Enddate { get; set; }

    public string? Currentstep { get; set; }

    public string? Status { get; set; }
}
