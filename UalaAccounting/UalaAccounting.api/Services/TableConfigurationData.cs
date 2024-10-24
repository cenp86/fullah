using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class TableConfigurationData : ITableConfigurationData
	{
        private readonly ContaContext _dbContext;
        private readonly ILogger<DbLogger> _logger;

        public TableConfigurationData(ContaContext dbContext, ILogger<DbLogger> logger)
		{
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Sheetparametrization>> GetReclassificationRules()
        {
            return await _dbContext.Sheetparametrizations.Where(t => t.Enable == true).ToListAsync();
        }

        public async Task<List<Productaccountinghub>> GetProductList()
        {
            return await _dbContext.Productaccountinghubs.Where(t => t.Enable == true).ToListAsync();
        }        
    }
}