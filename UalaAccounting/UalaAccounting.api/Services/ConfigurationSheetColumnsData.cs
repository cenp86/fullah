using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class ConfigurationSheetColumnsData : IConfigurationSheetColumnsData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<ConfigurationSheetColumnsData> _logger;

        public ConfigurationSheetColumnsData(ContaContext dbContext, ILogger<ConfigurationSheetColumnsData> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<List<Configurationsheetcolumn>> GetConfigurationBySheetAsync(string sheet)
        {
            try
            {
                return await _dbContext.Configurationsheetcolumns.Where(x => x.Enable == true && x.Configurationsheetencodedkey == sheet).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Configurationsheetcolumn>> GetConfigurationByListSheetAsync(List<string> list)
        {
            try
            {
                return await _dbContext.Configurationsheetcolumns.Where(x => x.Enable == true && list.Any(o => o == x.Configurationsheetencodedkey)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

