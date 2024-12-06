using System;
using UalaAccounting.api.EntitiesDB;
using Microsoft.EntityFrameworkCore;

namespace UalaAccounting.api.Services
{
    public class AccountingEntriesData : IAccountingEntriesData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<DbLogger> _logger;
        private readonly IDbContextFactory<ContaContext> _contextFactory;

        public AccountingEntriesData(ContaContext dbContext, ILogger<DbLogger> logger, IDbContextFactory<ContaContext> contextFactory)
        {
            _dbContext = dbContext;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task<List<Accountinghubentry>> GetAccountingEntriesAsync(DateTime from, DateTime to)
        {
            using var _dbContext = _contextFactory.CreateDbContext();

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
            using var _dbContext = _contextFactory.CreateDbContext();

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
            using var _dbContext = _contextFactory.CreateDbContext();

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
            using var _dbContext = _contextFactory.CreateDbContext();

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
            using var _dbContext = _contextFactory.CreateDbContext();

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
            using var _dbContext = _contextFactory.CreateDbContext();
            
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

        public async Task DeleteBalancesEntriesAsync(DateTime from, DateTime to)
        {
            using var _dbContext = _contextFactory.CreateDbContext();

            try
            {
                var newEntriesToDelete = await _dbContext.Accountingbalancestage3s.Where(x => x.Creationdate >= from && x.Creationdate < to).ToListAsync();

                _dbContext.Accountingbalancestage3s.RemoveRange(newEntriesToDelete);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task BalancesEntriesInsertBatchAsync(List<Accountingbalancestage3> list)
        {
            using var _dbContext = _contextFactory.CreateDbContext();

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
    }
}