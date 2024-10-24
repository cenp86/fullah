using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Loanaccounthistory
{
    public string Encodedkey { get; set; } = null!;

    public string Accountholderkey { get; set; } = null!;

    public string Accountholdertype { get; set; } = null!;

    public string? Accountstate { get; set; }

    public string? Accountsubstate { get; set; }

    public string? Rescheduledaccountkey { get; set; }

    public string? Assignedbranchkey { get; set; }

    public string? Assigneduserkey { get; set; }

    public DateTime? Closeddate { get; set; }

    public DateTime? Lastlockeddate { get; set; }

    public DateTime? Creationdate { get; set; }

    public DateTime? Approveddate { get; set; }

    public decimal? Feesdue { get; set; }

    public decimal? Feespaid { get; set; }

    public int Graceperiod { get; set; }

    public string? Graceperiodtype { get; set; }

    public string? Id { get; set; }

    public string? Interestcalculationmethod { get; set; }

    public string? Interesttype { get; set; }

    public string? Repaymentschedulemethod { get; set; }

    public string? Interestapplicationmethod { get; set; }

    public string? Paymentmethod { get; set; }

    public string? Interestchargefrequency { get; set; }

    public decimal? Interestbalance { get; set; }

    public decimal? Interestpaid { get; set; }

    public decimal? Interestrate { get; set; }

    public DateTime? Lastmodifieddate { get; set; }

    public DateTime? Lastsettoarrearsdate { get; set; }

    public decimal? Loanamount { get; set; }

    public decimal Periodicpayment { get; set; }

    public string? LoangroupEncodedkeyOid { get; set; }

    public string? Loanname { get; set; }

    public string? Notes { get; set; }

    public decimal? Penaltydue { get; set; }

    public decimal? Penaltypaid { get; set; }

    public decimal? Principalbalance { get; set; }

    public decimal? Principalpaid { get; set; }

    public string? Producttypekey { get; set; }

    public int Repaymentinstallments { get; set; }

    public int? Repaymentperiodcount { get; set; }

    public string? Repaymentperiodunit { get; set; }

    public int? AccountsIntegerIdx { get; set; }

    public string? Migrationeventkey { get; set; }

    public string? Assignedcentrekey { get; set; }

    public DateTime? Lastaccountappraisaldate { get; set; }

    public int? Principalrepaymentinterval { get; set; }

    public decimal? Principaldue { get; set; }

    public decimal? Interestdue { get; set; }

    public DateTime? Lastinterestreviewdate { get; set; }

    public ulong Accruelateinterest { get; set; }

    public decimal? Interestspread { get; set; }

    public string Interestratesource { get; set; } = null!;

    public string? Interestratereviewunit { get; set; }

    public int? Interestratereviewcount { get; set; }

    public decimal? Accruedinterest { get; set; }

    public DateTime? Lastinterestapplieddate { get; set; }

    public decimal? Feesbalance { get; set; }

    public decimal? Penaltybalance { get; set; }

    public string Scheduleduedatesmethod { get; set; } = null!;

    public ulong Hascustomschedule { get; set; }

    public byte[]? Fixeddaysofmonth { get; set; }

    public string? Shortmonthhandlingmethod { get; set; }

    public decimal? Taxrate { get; set; }

    public DateTime? Lasttaxratereviewdate { get; set; }

    public decimal? Penaltyrate { get; set; }

    public string? Loanpenaltycalculationmethod { get; set; }

    public decimal? Accruedpenalty { get; set; }

    public string? Activationtransactionkey { get; set; }

    public string? Lineofcreditkey { get; set; }

    public byte[]? Lockedoperations { get; set; }

    public decimal? Interestcommission { get; set; }

    public decimal? Defaultfirstrepaymentduedateoffset { get; set; }

    public string? Principalpaymentsettingskey { get; set; }

    public string? Interestbalancecalculationmethod { get; set; }

    public string? Disbursementdetailskey { get; set; }

    public int Arrearstoleranceperiod { get; set; }

    public ulong Accrueinterestaftermaturity { get; set; }

    public string? Prepaymentrecalculationmethod { get; set; }

    public string? Principalpaidinstallmentstatus { get; set; }

    public string? Elementsrecalculationmethod { get; set; }

    public string? Latepaymentsrecalculationmethod { get; set; }

    public string? Applyinterestonprepaymentmethod { get; set; }

    public ulong Allowoffset { get; set; }

    public string Futurepaymentsacceptance { get; set; } = null!;

    public decimal? Redrawbalance { get; set; }

    public string? Prepaymentacceptance { get; set; }

    public decimal? Interestfromarrearsaccrued { get; set; }

    public decimal? Interestfromarrearsdue { get; set; }

    public decimal? Interestfromarrearspaid { get; set; }

    public decimal? Interestfromarrearsbalance { get; set; }

    public string? Interestroundingversion { get; set; }

    public string? Accountarrearssettingskey { get; set; }

    public decimal Holdbalance { get; set; }

    public DateOnly Snapshotdate { get; set; }
}
