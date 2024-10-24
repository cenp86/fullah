using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class ConfigurationSheetsData : IConfigurationSheetsData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<ConfigurationSheetsData> _logger;

        public ConfigurationSheetsData(ContaContext dbContext, ILogger<ConfigurationSheetsData> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Configurationsheet>> GetConfigurationSheetsAsync()
        {
            try
            {
                return await _dbContext.Configurationsheets.Where(x => x.Enable == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

