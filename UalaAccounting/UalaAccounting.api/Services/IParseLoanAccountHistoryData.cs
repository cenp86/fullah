using System;
using UalaAccounting.api.EntitiesDB;
namespace UalaAccounting.api.Services
{
    public interface IParseLoanAccountHistoryData
    {
        Task GetParseLoanAccountHistoryDataAsync(DateOnly today);
        Task DeleteLoanAccountHistoryRecords(DateOnly today);
        Task<Loanaccounthistory> GetLoanAccountHistoryRecord(String accountId, DateOnly recordDate);
    }
}