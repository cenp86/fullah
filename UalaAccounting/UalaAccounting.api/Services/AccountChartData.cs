using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class AccountChartData : IAccountChartData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<AccountChartData> _logger;

        public AccountChartData(ContaContext dbContext, ILogger<AccountChartData> logger)
		{
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<string>> GetAccountChartByNameAsync()
        {
            try
            {
                return await _dbContext.Accountcharts.Select(x => x.Accountchartid).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Accountchart>> GetAccountChartByIdAsync(string accountChartId)
        {
            try
            {
                return await _dbContext.Accountcharts.Where(x => x.Accountchartid == accountChartId && x.Enable == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveAccountChartFromList(List<Accountchart> list)
        {
            try
            {
                await _dbContext.Accountcharts.AddRangeAsync(list);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateAccountChartFromList(List<Accountchart> list)
        {
            try
            {
                foreach (var item in list)
                {
                    _dbContext.Entry(item).State = EntityState.Modified;
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

