using System;
using UalaAccounting.api.EntitiesDB;
using UalaAccounting.api.Models;
using UalaAccounting.api.Services;

namespace UalaAccounting.api.ApplicationCore
{
    public class LoanAccountHistoryProcess : ILoanAccountHistoryProcess
    {
        private readonly ILogger<BusinessLogic> logger;
        private readonly IParseLoanAccountHistoryData parseLoanAccountHistoryData;

        public LoanAccountHistoryProcess(ILogger<BusinessLogic> _logger, IParseLoanAccountHistoryData _parseLoanAccountHistoryData)
        {
            logger = _logger;
            parseLoanAccountHistoryData = _parseLoanAccountHistoryData;
        }

        public async Task BuildLoanAccountHistory()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            try
            {
                await parseLoanAccountHistoryData.DeleteLoanAccountHistoryRecords(today);

                await parseLoanAccountHistoryData.GetParseLoanAccountHistoryDataAsync(today);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}