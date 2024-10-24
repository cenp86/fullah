using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class CustomQueryInDatabaseServices : ICustomQueryInDatabaseServices
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<CustomQueryInDatabaseServices> _logger;

        public CustomQueryInDatabaseServices(ContaContext dbContext, ILogger<CustomQueryInDatabaseServices> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ExecuteDatabaseCommand(string strQuery)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
                _dbContext.Database.ExecuteSqlRaw(strQuery);
            }
            catch (Exception exc)
            {
                _logger.LogError($"An error occurred during the process 'ExecuteDatabaseCommand' -- module: {exc} -- query: {strQuery}");
                throw;
            }
        }
    }
}

