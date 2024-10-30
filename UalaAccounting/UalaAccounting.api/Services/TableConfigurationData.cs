using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class TableConfigurationData : ITableConfigurationData
	{
        private readonly ContaContext _dbContext;
        private readonly ILogger<DbLogger> _logger;
        private readonly IDbContextFactory<ContaContext> _contextFactory;

        public TableConfigurationData(ContaContext dbContext, ILogger<DbLogger> logger, IDbContextFactory<ContaContext> contextFactory)
		{
            _dbContext = dbContext;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task<List<Sheetparametrization>> GetReclassificationRules()
        {
            using var _dbContext = _contextFactory.CreateDbContext();

            return await _dbContext.Sheetparametrizations.Where(t => t.Enable == true).ToListAsync();
        }

        public async Task<List<Productaccountinghub>> GetProductList()
        {
            using var _dbContext = _contextFactory.CreateDbContext();

            return await _dbContext.Productaccountinghubs.Where(t => t.Enable == true).ToListAsync();
        }        
    }
}