using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Loantransaction
{
    public string Encodedkey { get; set; } = null!;

    public decimal? Amount { get; set; }

    public decimal? Balance { get; set; }

    public string? Branchkey { get; set; }

    public string? Centrekey { get; set; }

    public string? Comment { get; set; }

    public string? DetailsEncodedkeyOid { get; set; }

    public string? Parentaccountkey { get; set; }

    public DateTime? Creationdate { get; set; }

    public long Transactionid { get; set; }

    public string? Type { get; set; }

    public string? Userkey { get; set; }

    public string? Reversaltransactionkey { get; set; }

    public DateTime? Entrydate { get; set; }

    public decimal? Principalamount { get; set; }

    public decimal? Interestamount { get; set; }

    public decimal Fundersinterestamount { get; set; }

    public decimal Organizationcommissionamount { get; set; }

    public decimal? Feesamount { get; set; }

    public decimal? Penaltyamount { get; set; }

    public string? IndexinterestrateEncodedkeyOid { get; set; }

    public string? TaxrateEncodedkeyOid { get; set; }

    public decimal? Taxoninterestamount { get; set; }

    public decimal? Deferredinterestamount { get; set; }

    public decimal? Deferredtaxoninterestamount { get; set; }

    public string? Migrationeventkey { get; set; }

    public decimal? Taxonfeesamount { get; set; }

    public decimal? Taxonpenaltyamount { get; set; }

    public string? Producttypekey { get; set; }

    public string? Parentloantransactionkey { get; set; }

    public string? Originalcurrencycode { get; set; }

    public decimal? Originalamount { get; set; }

    public string? Tillkey { get; set; }

    public decimal? Interestrate { get; set; }

    public string? Loantransactiontermskey { get; set; }

    public decimal? Redrawbalance { get; set; }

    public decimal? Principalbalance { get; set; }

    public decimal? Advanceposition { get; set; }

    public decimal? Arrearsposition { get; set; }

    public decimal? Expectedprincipalredraw { get; set; }

    public decimal? Interestfromarrearsamount { get; set; }

    public decimal? Taxoninterestfromarrearsamount { get; set; }

    public string? Externalid { get; set; }

    public virtual Branch? BranchkeyNavigation { get; set; }

    public virtual Transactiondetail? DetailsEncodedkeyO { get; set; }

    public virtual ICollection<Loantransaction> InverseParentloantransactionkeyNavigation { get; set; } = new List<Loantransaction>();

    public virtual ICollection<Loantransaction> InverseReversaltransactionkeyNavigation { get; set; } = new List<Loantransaction>();

    public virtual ICollection<Loanaccount> Loanaccounts { get; set; } = new List<Loanaccount>();

    public virtual Loanaccount? ParentaccountkeyNavigation { get; set; }

    public virtual Loantransaction? ParentloantransactionkeyNavigation { get; set; }

    public virtual Loanproduct? ProducttypekeyNavigation { get; set; }

    public virtual Loantransaction? ReversaltransactionkeyNavigation { get; set; }
}
