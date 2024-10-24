using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class ConfigurationData : IConfigurationData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<ConfigurationData> _logger;

        public ConfigurationData(ContaContext dbContext, ILogger<ConfigurationData> logger)
		{
            _logger = logger;
            _dbContext = dbContext;
		}

        public async Task<List<Configurationaccountinghub>> GetConfigurationEnableAsync()
        {
            try
            {
                return await _dbContext.Configurationaccountinghubs.Where(x => x.Enable == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
	}
}

