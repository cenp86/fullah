using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class ProductAccountingHubData : IProductAccountingHubData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<ProductAccountingHubData> _logger;

        public ProductAccountingHubData(ContaContext dbContext, ILogger<ProductAccountingHubData> logger)
		{
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Productaccountinghub>> GetProducctByAccountChart(string accountChartId)
        {
            try
            {
                return await _dbContext.Productaccountinghubs.Where(x => x.Accountchart == accountChartId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateProductsFromList(List<Productaccountinghub> list)
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

        public async Task SaveProductsFromList(List<Productaccountinghub> list)
        {
            try
            {
                await _dbContext.Productaccountinghubs.AddRangeAsync(list);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

