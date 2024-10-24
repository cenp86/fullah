using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
    public interface IAccountingEntriesData
    {
        Task<List<Accountinghubentry>> GetAccountingEntriesAsync(DateTime from, DateTime to);
        Task<List<Accountinghubentry>> GetAdjustmentEntriesAsync(DateTime from, DateTime to);
        Task AccountingEntriesInsertBatchAsync(List<Accountinghubexit> list);
        Task DeleteNewEntriesAsync(DateTime from, DateTime to);
        Task<List<Accountinghubexit>> getOriginalEntriesFromAdjustedAsync(string originalTransactionKey, string glCode);
        Task<Loanaccounthistory> getAccountDetailsAsync(string loanid);
    }
}

