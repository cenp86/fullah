using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Loanproduct
{
    public string Encodedkey { get; set; } = null!;

    public string? Id { get; set; }

    public ulong? Activated { get; set; }

    public DateTime? Creationdate { get; set; }

    public int? Defaultgraceperiod { get; set; }

    public decimal? Defaultloanamount { get; set; }

    public int? Defaultnuminstallments { get; set; }

    public int? Defaultrepaymentperiodcount { get; set; }

    public string? Graceperiodtype { get; set; }

    public string? Interestcalculationmethod { get; set; }

    public string? Interesttype { get; set; }

    public string Repaymentschedulemethod { get; set; } = null!;

    public DateTime? Lastmodifieddate { get; set; }

    public int? Maxgraceperiod { get; set; }

    public decimal? Maxloanamount { get; set; }

    public int? Maxnuminstallments { get; set; }

    public decimal? Maxpenaltyrate { get; set; }

    public int? Mingraceperiod { get; set; }

    public decimal? Minloanamount { get; set; }

    public int? Minnuminstallments { get; set; }

    public decimal? Minpenaltyrate { get; set; }

    public decimal? Defaultpenaltyrate { get; set; }

    public string? Productdescription { get; set; }

    public string? Productname { get; set; }

    public string? Repaymentperiodunit { get; set; }

    public string? Prepaymentacceptance { get; set; }

    public string Interestapplicationmethod { get; set; } = null!;

    public ulong Allowarbitraryfees { get; set; }

    public int? Defaultprincipalrepaymentinterval { get; set; }

    public string? Loanpenaltycalculationmethod { get; set; }

    public int? Loanpenaltygraceperiod { get; set; }

    public string? Repaymentcurrencyrounding { get; set; }

    public string? Roundingrepaymentschedulemethod { get; set; }

    public string? Prepaymentrecalculationmethod { get; set; }

    public string? Principalpaidinstallmentstatus { get; set; }

    public string? Daysinyear { get; set; }

    public string? Scheduleinterestdayscountmethod { get; set; }

    public byte[]? Repaymentallocationorder { get; set; }

    public string Idgeneratortype { get; set; } = null!;

    public string Idpattern { get; set; } = null!;

    public string? Accountingmethod { get; set; }

    public ulong Accountlinkingenabled { get; set; }

    public string? Linkablesavingsproductkey { get; set; }

    public ulong Autolinkaccounts { get; set; }

    public ulong Autocreatelinkedaccounts { get; set; }

    public byte[]? Repaymentscheduleeditoptions { get; set; }

    public string Scheduleduedatesmethod { get; set; } = null!;

    public string Paymentmethod { get; set; } = null!;

    public byte[]? Fixeddaysofmonth { get; set; }

    public string? Shortmonthhandlingmethod { get; set; }

    public ulong Taxesoninterestenabled { get; set; }

    public string? Taxsourcekey { get; set; }

    public string? Taxcalculationmethod { get; set; }

    public string Futurepaymentsacceptance { get; set; } = null!;

    public ulong Taxesonfeesenabled { get; set; }

    public ulong Taxesonpenaltyenabled { get; set; }

    public string? Applyinterestonprepaymentmethod { get; set; }

    public string Repaymentelementsroundingmethod { get; set; } = null!;

    public string? Elementsrecalculationmethod { get; set; }

    public int? Dormancyperioddays { get; set; }

    public int? Lockperioddays { get; set; }

    public string? Cappingmethod { get; set; }

    public string? Cappingconstrainttype { get; set; }

    public decimal? Cappingpercentage { get; set; }

    public ulong Cappingapplyaccruedchargesbeforelocking { get; set; }

    public string Settlementoptions { get; set; } = null!;

    public decimal? Offsetpercentage { get; set; }

    public string Accountinitialstate { get; set; } = null!;

    public int? Maxnumberofdisbursementtranches { get; set; }

    public string Latepaymentsrecalculationmethod { get; set; } = null!;

    public string? Amortizationmethod { get; set; }

    public string? Interestratesettingskey { get; set; }

    public string Lineofcreditrequirement { get; set; } = null!;

    public string? Productsecuritysettingskey { get; set; }

    public decimal? Defaultfirstrepaymentduedateoffset { get; set; }

    public decimal? Minfirstrepaymentduedateoffset { get; set; }

    public decimal? Maxfirstrepaymentduedateoffset { get; set; }

    public string Interestaccruedaccountingmethod { get; set; } = null!;

    public string Loanproducttype { get; set; } = null!;

    public ulong Forindividuals { get; set; }

    public ulong Forpuregroups { get; set; }

    public ulong Forhybridgroups { get; set; }

    public ulong Forallbranches { get; set; }

    public string? Principalpaymentsettingskey { get; set; }

    public string Repaymentreschedulingmethod { get; set; } = null!;

    public string? Interestbalancecalculationmethod { get; set; }

    public string? Arrearssettingskey { get; set; }

    public string? Redrawsettingskey { get; set; }

    public ulong Allowcustomrepaymentallocation { get; set; }

    public ulong Accruelateinterest { get; set; }

    public string? Interestaccrualcalculation { get; set; }

    public string? Category { get; set; }

    public string? Currencycode { get; set; }

    public ulong Foureyesprincipleloanapproval { get; set; }

    public virtual ICollection<Loanaccount> Loanaccounts { get; set; } = new List<Loanaccount>();

    public virtual ICollection<Loantransaction> Loantransactions { get; set; } = new List<Loantransaction>();
}
