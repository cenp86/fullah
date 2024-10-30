using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class ConfigurationData : IConfigurationData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<ConfigurationData> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDbContextFactory<ContaContext> _contextFactory;

        public ConfigurationData(ContaContext dbContext, ILogger<ConfigurationData> logger, IServiceScopeFactory serviceScopeFactory, IDbContextFactory<ContaContext> contextFactory)
		{
            _logger = logger;
            _dbContext = dbContext;
            _serviceScopeFactory = serviceScopeFactory;
            _contextFactory = contextFactory;
		}

        public async Task<List<Configurationaccountinghub>> GetConfigurationEnableAsync()
        {
            try
            {               
                using var _dbContext = _contextFactory.CreateDbContext();

                return await _dbContext.Configurationaccountinghubs.Where(x => x.Enable == true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
	}
}

