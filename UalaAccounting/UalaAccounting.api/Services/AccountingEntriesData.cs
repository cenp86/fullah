using System;
using UalaAccounting.api.EntitiesDB;
using Microsoft.EntityFrameworkCore;

namespace UalaAccounting.api.Services
{
    public class AccountingEntriesData : IAccountingEntriesData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<DbLogger> _logger;

        public AccountingEntriesData(ContaContext dbContext, ILogger<DbLogger> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Accountinghubentry>> GetAccountingEntriesAsync(DateTime from, DateTime to)
        {
            try
            {
                return await _dbContext.Accountinghubentries
                    .Where(x => x.Creationdate >= from && x.Creationdate < to && !x.Loantransactiontype.Contains("ADJUSTMENT"))
                    .OrderBy(x => x.Creationdate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<Accountinghubentry>> GetAdjustmentEntriesAsync(DateTime from, DateTime to)
        {
            try
            {
                return await _dbContext.Accountinghubentries
                    .Where(x => x.Creationdate >= from && x.Creationdate < to && x.Loantransactiontype.Contains("ADJUSTMENT"))
                    .OrderBy(x => x.Creationdate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task AccountingEntriesInsertBatchAsync(List<Accountinghubexit> list)
        {
            try
            {
                await _dbContext.AddRangeAsync(list);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<Accountinghubexit>> getOriginalEntriesFromAdjustedAsync(string originalTransactionId, string glCode)
        {
            try
            {
                return await _dbContext.Accountinghubexits.Where(x => x.Transactionid == originalTransactionId && x.Mambuglcode == glCode).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task DeleteNewEntriesAsync(DateTime from, DateTime to)
        {
            try
            {
                var newEntriesToDelete = await _dbContext.Accountinghubexits.Where(x => x.Creationdate >= from && x.Creationdate < to).ToListAsync();

                _dbContext.Accountinghubexits.RemoveRange(newEntriesToDelete);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<Loanaccounthistory> getAccountDetailsAsync(string loanid)
        {
            try
            {
                var data = await _dbContext.Loanaccounthistories.Where(x => x.Id == loanid).ToListAsync();

                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}