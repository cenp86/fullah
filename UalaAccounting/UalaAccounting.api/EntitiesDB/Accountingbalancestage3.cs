using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Accountingbalancestage3
{
    public string Loanid { get; set; } = null!;

    public decimal? VarInterestS3 { get; set; }

    public decimal? VarInterestMa { get; set; }

    public decimal? VarPenaltyS3 { get; set; }

    public decimal? VarPenaltyMa { get; set; }

    public DateTime Creationdate { get; set; }
}
