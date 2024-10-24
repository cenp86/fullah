using System;
namespace UalaAccounting.api.Services
{
    public interface IParseLoanAccountHistoryData
    {
        Task GetParseLoanAccountHistoryDataAsync(DateOnly today);
        Task DeleteLoanAccountHistoryRecords(DateOnly today);
    }
}